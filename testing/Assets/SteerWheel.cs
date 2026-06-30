using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerWheel : MonoBehaviour
{
    public float rotationSpeed = 500f;
    public float maxSteerAngle = 35f; // 最大转向角度
    public float steerSpeed = 100f;   // 转向平滑速度

    private float currentSteerAngle = 0f;

    void Update()
    {
        // 1. 处理轮子自身的滚动 (同步 W/S)
        if (Input.GetKey(KeyCode.W))
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
        else if (Input.GetKey(KeyCode.S))
            transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime, Space.Self);

        // 2. 处理转向 (A/D)
        if (Input.GetKey(KeyCode.A))
            currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, -maxSteerAngle, steerSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.D))
            currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, maxSteerAngle, steerSpeed * Time.deltaTime);
        else
            currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, 0, steerSpeed * Time.deltaTime); // 回正

        // 应用转向角度到轮子（假设轮子默认朝向是 Forward）
        // 注意：这里改变的是轮子的本地旋转 (LocalRotation)
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, currentSteerAngle, transform.localRotation.eulerAngles.z);
    }
}