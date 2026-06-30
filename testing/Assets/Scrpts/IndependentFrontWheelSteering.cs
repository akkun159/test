using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependentFrontWheelSteering : MonoBehaviour
{
    [Header("转向设置")]
    [Tooltip("此轮子的最大偏转角度")]
    [SerializeField] private float maxSteerAngle = 35f;

    [Tooltip("转向的平滑速度 (度/秒)")]
    [SerializeField] private float steerSpeed = 150f;

    [Header("镜像设置")]
    [Tooltip("如果是右侧轮子且转向反了，请勾选此项。")]
    [SerializeField] private bool invertSteering = false;

    // 记录初始的本地旋转，防止转回来时产生累积误差
    private Quaternion initialLocalRotation;
    // 当前的转向目标角度
    private float currentSteerAngle = 0f;

    void Start()
    {
        // 初始化时记录轮子原本的旋转状态（确保轮子初始是向前的）
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        // 1. 获取转向输入 (A = -1, D = 1, 无按键 = 0)
        float steerInput = Input.GetAxisRaw("Horizontal");

        // 2. 处理镜像
        if (invertSteering)
        {
            steerInput *= -1f;
        }

        // 3. 平滑地计算目标角度
        float targetAngle = steerInput * maxSteerAngle;
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);

        // 4. 应用旋转 (绕自身的 Y 轴)
        // 我们基于初始旋转进行偏转，这样不会影响可能存在的滚动（X轴旋转）
        transform.localRotation = initialLocalRotation * Quaternion.Euler(0f, currentSteerAngle, 0f);
    }
}