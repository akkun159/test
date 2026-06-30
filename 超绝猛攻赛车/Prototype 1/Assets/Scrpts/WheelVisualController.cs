using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelVisualController : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private CarMovement carMovement; // 把你的 CarMovement 脚本拖进来

    [Header("轮子设置")]
    [SerializeField] private Transform[] leftWheels;
    [SerializeField] private Transform[] rightWheels;

    [Header("滚动系数")]
    [SerializeField] private float rotationMultiplier = 50f; // 调节轮子滚动的快慢

    void Update()
    {
        // 核心：直接获取车子的速度，而不是获取按键
        // 车子越快，currentSpeed 越大，轮子转得越快
        float speed = carMovement.GetCurrentSpeed();

        // 如果速度不为0，转动轮子
        if (Mathf.Abs(speed) > 0.1f)
        {
            RotateWheels(speed);
        }
    }

    private void RotateWheels(float speed)
    {
        // 计算旋转角度：速度 * 系数 * 时间
        float angle = speed * rotationMultiplier * Time.deltaTime;

        foreach (Transform wheel in leftWheels)
        {
            wheel.Rotate(Vector3.right * angle);
        }

        foreach (Transform wheel in rightWheels)
        {
            // 注意：如果镜像方向反了，这里改为 Vector3.right
            wheel.Rotate(Vector3.left * angle);
        }
    }
}