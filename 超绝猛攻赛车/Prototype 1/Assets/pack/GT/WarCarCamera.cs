using UnityEngine;

public class WarCarCamera : MonoBehaviour
{
    [Header("追踪目标")]
    public Transform targetCar;
    public Rigidbody targetRigidbody;

    [Header("弹簧阻尼跟随参数")]
    public float distance = 6.0f;
    public float height = 2.5f;
    public float positionDamping = 10f;
    public float rotationDamping = 5f;

    [Header("极速感 FOV 动态变化")]
    public float baseFOV = 60f;
    public float maxFOV = 85f;
    public float speedForMaxFOV = 30f; // 达到此速度(m/s)时FOV拉到最大

    // 震屏参数
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targetCar == null || targetRigidbody == null) return;

        HandleFollow();
        HandleSpeedFOV();
        HandleShake();
    }

    private void HandleFollow()
    {
        // 计算目标位置（车后方上方）
        Vector3 desiredPosition = targetCar.position - targetCar.forward * distance + Vector3.up * height;

        // 平滑插值移动
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);

        // 平滑插值旋转（看向车头前方一点）
        Vector3 lookAtTarget = targetCar.position + targetCar.forward * 5f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationDamping * Time.deltaTime);
    }

    private void HandleSpeedFOV()
    {
        // 【清单4：极速感】根据车速动态拉伸视野，速度越快越有视觉冲击力
        float currentSpeed = targetRigidbody.velocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / speedForMaxFOV);
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 5f);

        if (speedRatio > 0.8f)
        {
            // 占位逻辑：高速时可以激活屏幕边缘的速度线特效粒子
            // Debug.Log("[VFX] 激活全屏速度线特效");
        }
    }

    private void HandleShake()
    {
        // 【清单5：碰撞震屏处理逻辑】
        if (shakeDuration > 0)
        {
            transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 公共方法：触发摄像机震动
    /// </summary>
    /// <param name="intensity">震动强度</param>
    public void TriggerShake(float intensity)
    {
        shakeDuration = 0.3f; // 震动时长 0.3秒
        shakeMagnitude = Mathf.Clamp(intensity, 0.1f, 1.5f); // 限制最大震动幅度
    }
}