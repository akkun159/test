using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("每秒旋转的最大角度 (1.5圈 = 540度)")]
    [SerializeField] private float maxRotationAngle = 540f;
    [SerializeField] private float steeringSpeed = 360f; // 转向速度
    [SerializeField] private float returnSpeed = 720f;   // 快速归位速度

    private float currentAngle = 0f; // 当前累计的旋转角度
    private Quaternion initialLocalRotation;

    void Start()
    {
        // 记录方向盘初始的本地旋转
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal"); // A为-1, D为1

        if (Mathf.Abs(input) > 0.01f)
        {
            // 如果有按键输入，按方向旋转
            currentAngle -= input * steeringSpeed * Time.deltaTime;
        }
        else
        {
            // 如果松开按键，向0点方向快速归位
            currentAngle = Mathf.MoveTowards(currentAngle, 0f, returnSpeed * Time.deltaTime);
        }

        // 限制旋转角度：1.5圈 = 540度
        currentAngle = Mathf.Clamp(currentAngle, -maxRotationAngle, maxRotationAngle);

        // 应用旋转 (假设方向盘是绕Z轴旋转的，如果你的模型轴向不同，请修改下方的 Vector3.forward)
        transform.localRotation = initialLocalRotation * Quaternion.Euler(0, 0, currentAngle);
    }
}