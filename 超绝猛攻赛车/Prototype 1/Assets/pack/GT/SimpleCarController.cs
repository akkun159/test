using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCarController : MonoBehaviour
{
    public enum ModelAxis { Z_Forward, Y_Up, X_Right, Negative_Z, Negative_Y, Negative_X }

    [Header("🛠️ 模型轴向纠正 (防飞天神器)")]
    [Tooltip("如果按W车往天上飞，请尝试更改此选项，直到车往前开")]
    public ModelAxis realForwardAxis = ModelAxis.Z_Forward;

    [Header("🏎️ 性能参数 (霸道控制)")]
    [Tooltip("最高时速 (160 km/h)")]
    public float topSpeedKmh = 160f;
    [Tooltip("0-100 km/h 加速时间 (13秒)")]
    public float timeTo100Kmh = 13f;
    [Tooltip("转弯灵敏度")]
    public float turnSpeed = 60f;

    [Header("🔧 四个柱体轮子 (视觉槽位)")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;
    [Tooltip("轮子摆头的最大角度")]
    public float maxWheelTurnAngle = 30f;

    // 内部计算变量
    private Rigidbody rb;
    private float currentSpeedMs = 0f; // 当前速度 (米/秒)
    private float topSpeedMs;          // 最高速度 (米/秒)
    private float acceleration;        // 加速度 (米/秒²)

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 强行锁死重心，防止任何翻车可能
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        // 换算单位：100 km/h = 27.77 m/s, 160 km/h = 44.44 m/s
        topSpeedMs = topSpeedKmh / 3.6f;
        // 计算严格的加速度：27.77 m/s 除以 13秒
        acceleration = (100f / 3.6f) / timeTo100Kmh;
    }

    void FixedUpdate()
    {
        // 1. 抓取最原始的键盘信号
        bool keyW = Input.GetKey(KeyCode.W);
        bool keyS = Input.GetKey(KeyCode.S);
        bool keyA = Input.GetKey(KeyCode.A);
        bool keyD = Input.GetKey(KeyCode.D);

        // ----------------- 动力系统：绝对受控的速度插值 -----------------
        if (keyW)
        {
            // 按W：平滑加速到最高速度
            currentSpeedMs = Mathf.MoveTowards(currentSpeedMs, topSpeedMs, acceleration * Time.fixedDeltaTime);
        }
        else if (keyS)
        {
            if (currentSpeedMs > 0.1f)
            {
                // 如果还在往前跑，按S就是两倍力量刹车
                currentSpeedMs = Mathf.MoveTowards(currentSpeedMs, 0f, acceleration * 2f * Time.fixedDeltaTime);
            }
            else
            {
                // 停下来了，按S就是倒车（最高倒车速度设定为极速的30%）
                currentSpeedMs = Mathf.MoveTowards(currentSpeedMs, -topSpeedMs * 0.3f, acceleration * Time.fixedDeltaTime);
            }
        }
        else
        {
            // 没按键，慢慢滑行停下
            currentSpeedMs = Mathf.MoveTowards(currentSpeedMs, 0f, acceleration * 0.5f * Time.fixedDeltaTime);
        }

        // --- 核心防飞天逻辑：获取真正的物理前方，并抹平Y轴 ---
        Vector3 moveDir = GetRealForward();
        moveDir.y = 0f; // 强制抹平Y轴，确保绝对不会飞向天空！

        if (moveDir.sqrMagnitude > 0.001f)
        {
            moveDir.Normalize();
        }
        else
        {
            // 如果获取的方向刚好是垂直的被抹平了，提供一个保底方向
            moveDir = transform.forward;
            moveDir.y = 0;
            moveDir.Normalize();
        }

        // 霸道覆写刚体速度！(保留了Y轴的重力掉落，XZ轴完全由脚本接管，绝不卡壳)
        rb.velocity = moveDir * currentSpeedMs + new Vector3(0, rb.velocity.y, 0);

        // ----------------- 转向系统：四轮反向打死 -----------------
        float steerAmount = 0f;
        if (keyA) steerAmount = -1f;
        if (keyD) steerAmount = 1f;

        // 只有车动起来才能转弯
        if (Mathf.Abs(currentSpeedMs) > 0.1f && steerAmount != 0f)
        {
            // 倒车时转弯反向
            float dir = Mathf.Sign(currentSpeedMs);
            float turnAngle = steerAmount * turnSpeed * dir * Time.fixedDeltaTime;

            // 霸道旋转车身 (基于世界坐标的Y轴，无论模型怎么躺，只在平地上左右转)
            Quaternion deltaRotation = Quaternion.Euler(0f, turnAngle, 0f);
            rb.MoveRotation(deltaRotation * rb.rotation);
        }

        // ----------------- 视觉表现：前后轮反向摆头 -----------------
        AnimateWheels(steerAmount);
    }

    // 根据下拉菜单选择，动态计算到底哪一面才是这辆车的“前方”
    private Vector3 GetRealForward()
    {
        switch (realForwardAxis)
        {
            case ModelAxis.Y_Up: return transform.up;
            case ModelAxis.X_Right: return transform.right;
            case ModelAxis.Negative_Z: return -transform.forward;
            case ModelAxis.Negative_Y: return -transform.up;
            case ModelAxis.Negative_X: return -transform.right;
            case ModelAxis.Z_Forward:
            default:
                return transform.forward;
        }
    }

    private void AnimateWheels(float steerInput)
    {
        // 目标角度：比如按 D(1)，前轮朝右转 30度，后轮朝左转 -30度
        float targetFrontAngle = steerInput * maxWheelTurnAngle;
        float targetRearAngle = -steerInput * maxWheelTurnAngle; // 后轮反向

        // 应用到四个轮子（使用局部坐标轴 LocalRotation）
        if (frontLeftWheel) frontLeftWheel.localRotation = Quaternion.Euler(0f, targetFrontAngle, 0f);
        if (frontRightWheel) frontRightWheel.localRotation = Quaternion.Euler(0f, targetFrontAngle, 0f);

        if (rearLeftWheel) rearLeftWheel.localRotation = Quaternion.Euler(0f, targetRearAngle, 0f);
        if (rearRightWheel) rearRightWheel.localRotation = Quaternion.Euler(0f, targetRearAngle, 0f);
    }
}