using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WarCarController : MonoBehaviour
{
    [Header("一、车辆结构 (Wheel Colliders)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("二、操控参数 (调参区)")]
    [Tooltip("最大引擎扭矩，决定起步是否暴躁")]
    public float maxMotorTorque = 2500f;
    [Tooltip("最大刹车力，决定能否瞬间刹停")]
    public float brakeTorque = 5000f;
    [Tooltip("最大转向角 (高速时)")]
    public float maxSteeringAngle = 35f;
    [Tooltip("低速转向加成，满足3个车身掉头")]
    public float lowSpeedSteerMultiplier = 1.5f;
    [Tooltip("重心偏移，向Y轴下方偏移防止翻车")]
    public Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0);

    [Header("三、手刹与漂移感")]
    [Tooltip("手刹时后轮侧向摩擦力的滑动系数 (越小越滑)")]
    public float handbrakeDriftMultiplier = 0.3f;
    private WheelFrictionCurve defaultRearSidewaysFriction;

    // 内部状态
    private Rigidbody rb;
    private float currentSteerAngle;
    private float currentAcceleration;
    private float currentBrakeForce;
    private bool isHandbraking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 【清单1：降低重心】保证漂移不翻车
        rb.centerOfMass = centerOfMassOffset;

        // 记录默认的轮胎侧向摩擦力，用于手刹恢复
        defaultRearSidewaysFriction = rearLeftWheel.sidewaysFriction;
    }

    void FixedUpdate()
    {
        HandleInput();
        ApplyMotor();
        ApplySteering();
        ApplyHandbrake();
        HandleVisualsAndVFX();
    }

    private void HandleInput()
    {
        // 获取老输入系统轴向 (W/S 和 A/D)
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // 【清单1：起步响应】无迟滞，直接给最大扭矩的比例
        currentAcceleration = verticalInput * maxMotorTorque;

        // 【清单3：刹车响应】按下反方向时施加巨大刹车力
        bool isBraking = (verticalInput > 0 && rb.velocity.z < -1f) || (verticalInput < 0 && rb.velocity.z > 1f);
        currentBrakeForce = isBraking ? brakeTorque : 0f;

        // 获取手刹状态
        isHandbraking = Input.GetKey(KeyCode.Space);

        // 【清单2：转向灵敏度】低速时加大转向角，高速时收紧
        float speedFactor = rb.velocity.magnitude / 30f; // 假设30为极速基准
        float dynamicSteerMultiplier = Mathf.Lerp(lowSpeedSteerMultiplier, 1f, speedFactor);
        currentSteerAngle = horizontalInput * maxSteeringAngle * dynamicSteerMultiplier;
    }

    private void ApplyMotor()
    {
        // 全时四驱，起步更暴躁
        frontLeftWheel.motorTorque = currentAcceleration;
        frontRightWheel.motorTorque = currentAcceleration;
        rearLeftWheel.motorTorque = currentAcceleration;
        rearRightWheel.motorTorque = currentAcceleration;

        // 施加刹车力
        frontLeftWheel.brakeTorque = currentBrakeForce;
        frontRightWheel.brakeTorque = currentBrakeForce;
        rearLeftWheel.brakeTorque = currentBrakeForce;
        rearRightWheel.brakeTorque = currentBrakeForce;
    }

    private void ApplySteering()
    {
        // 前轮转向
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    private void ApplyHandbrake()
    {
        if (isHandbraking)
        {
            // 锁死后轮
            rearLeftWheel.brakeTorque = brakeTorque;
            rearRightWheel.brakeTorque = brakeTorque;

            // 【清单6：漂移感】降低后轮侧向摩擦力，实现甩尾
            WheelFrictionCurve driftFriction = defaultRearSidewaysFriction;
            driftFriction.stiffness *= handbrakeDriftMultiplier;
            rearLeftWheel.sidewaysFriction = driftFriction;
            rearRightWheel.sidewaysFriction = driftFriction;
        }
        else
        {
            // 恢复正常抓地力
            rearLeftWheel.sidewaysFriction = defaultRearSidewaysFriction;
            rearRightWheel.sidewaysFriction = defaultRearSidewaysFriction;
        }
    }

    private void HandleVisualsAndVFX()
    {
        // 【清单6：漂移视觉占位】
        if (isHandbraking && rb.velocity.magnitude > 5f)
        {
            Debug.Log("[VFX] 滋滋滋！生成轮胎印与白烟粒子！");
        }
    }

    // 【清单5：碰撞反馈】监听物理碰撞
    private void OnCollisionEnter(Collision collision)
    {
        // 计算相对碰撞速度的强度
        float impactMagnitude = collision.relativeVelocity.magnitude;

        // 只有速度够大才触发反馈
        if (impactMagnitude > 5f)
        {
            ApplyCollisionFeedback(impactMagnitude);
        }
    }

    /// <summary>
    /// 公共方法：供外部调用或自身碰撞触发
    /// </summary>
    public void ApplyCollisionFeedback(float impactMagnitude)
    {
        Debug.Log($"[Audio/VFX] 砰！撞击墙壁，撞击力度: {impactMagnitude}");

        // 尝试获取摄像机脚本触发震屏
        WarCarCamera cam = Camera.main.GetComponent<WarCarCamera>();
        if (cam != null)
        {
            cam.TriggerShake(impactMagnitude * 0.1f);
        }
    }
}