using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveWheel : MonoBehaviour
{
    public float rotationSpeed = 500f; // 轮子转动的角速度
    public WheelData wheelData;       // 引用之前定义的 WheelData 脚本

    void Update()
    {
        // 直接获取键盘按键
        if (Input.GetKey(KeyCode.W))
        {
            // 向前转动
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // 向后转动
            transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
        }
    }
}