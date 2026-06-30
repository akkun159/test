using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour
{
    [Header("核心物理属性")]
    public float mass = 1500f; // 车辆总质量 (kg)
    public float dragCoefficient = 0.3f; // 空气阻力系数

    [Header("性能参数")]
    public float maxHorsepower = 200f; // 最大马力
    public float brakeForce = 2000f; // 制动力

    [Header("状态监控")]
    public Vector3 currentVelocity; // 当前速度向量
    public float currentSpeed; // 当前时速 (km/h)

    void Update()
    {
        // 简单的速度计算供参考
        currentSpeed = GetComponent<CharacterController>() ? 0 : 0; // 预留接口
    }
}