using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePitchEffect : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private CarMovement carMovement;

    [Header("俯仰设置")]
    [SerializeField] private float pitchMultiplier = 0.5f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float maxPitchAngle = 5f;

    private float lastSpeed = 0f;
    private float currentPitch = 0f;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (carMovement == null) return;

        float currentSpeed = carMovement.GetCurrentSpeed();
        float acceleration = (currentSpeed - lastSpeed) / Time.deltaTime;
        lastSpeed = currentSpeed;

        // 【修改点】：这里在 acceleration 前面加了负号
        // 这样起步加速时 (正加速度) 会变成负值，即车头下沉 (点头)
        // 刹车减速时 (负加速度) 会变成正值，即车头抬起 (翘头)
        float targetPitch = Mathf.Clamp(-acceleration * pitchMultiplier, -maxPitchAngle, maxPitchAngle);

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * returnSpeed);

        // 如果你希望是围绕 X 轴旋转，保持不变；如果发现还是反的，请尝试修改为 -currentPitch
        transform.localRotation = initialRotation * Quaternion.Euler(currentPitch, 0f, 0f);
    }
}