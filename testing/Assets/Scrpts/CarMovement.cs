using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("基础参数 (普通模式)")]
    [SerializeField] private float maxForwardSpeed = 180f;
    [SerializeField] private float maxBackwardSpeed = 80f;
    [SerializeField] private float accelerationTime = 11f;
    [SerializeField] private float frictionDeceleration = 2f;
    [SerializeField] private float brakeDeceleration = 12f;
    [SerializeField] private float turnSpeed = 100f;

    [Header("运动模式增益")]
    [SerializeField] private float sportAccelMultiplier = 1.5f; // 加速度提升 50%
    [SerializeField] private float sportTurnMultiplier = 1.3f;  // 转向灵敏度提升 30%

    [Header("氮气加速设置")]
    [SerializeField] private float boostMaxSpeed = 220f;
    [SerializeField] private float boostDuration = 4f;
    [SerializeField] private float boostCooldown = 5f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private float currentSpeed = 0f;

    // 状态变量
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isBoosting = false;
    private bool isSportMode = false; // 当前模式

    public float GetCurrentSpeed() => currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // 切换模式逻辑
        if (Input.GetKeyDown(KeyCode.P))
        {
            isSportMode = !isSportMode;
            Debug.Log(isSportMode ? "当前模式：运动模式" : "当前模式：普通模式");
        }

        HandleBoostInput();
    }

    void FixedUpdate()
    {
        if (boostTimer > 0) boostTimer -= Time.fixedDeltaTime;
        if (cooldownTimer > 0) cooldownTimer -= Time.fixedDeltaTime;

        bool isBraking = Input.GetKey(KeyCode.Space);
        if (isBraking && isBoosting) isBoosting = false;

        // 根据模式调整加速度系数
        float currentAccelTime = isSportMode ? (accelerationTime / sportAccelMultiplier) : accelerationTime;
        float acceleration = (100f / 3.6f) / currentAccelTime;

        // 目标速度与转向灵敏度调整
        float currentLimit = isBoosting ? boostMaxSpeed : (moveInput >= 0 ? maxForwardSpeed : maxBackwardSpeed);
        float targetSpeed = moveInput * (currentLimit / 3.6f);
        float currentTurnSpeed = isSportMode ? turnSpeed * sportTurnMultiplier : turnSpeed;

        // 加减速逻辑
        if (isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * brakeDeceleration * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(moveInput) > 0.01f)
        {
            float accelMultiplier = isBoosting ? 2f : 1f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * accelMultiplier * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * frictionDeceleration * Time.fixedDeltaTime);
        }

        // 应用移动
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

        // 转向逻辑
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / (180f / 3.6f));
            float baseTurnSpeed = currentTurnSpeed * Mathf.Lerp(0.3f, 1.0f, speedFactor);
            float dynamicInputFactor = (Mathf.Abs(moveInput) < 0.01f) ? 0.2f : 1.0f;

            float adjustedTurnInput = (currentSpeed < 0) ? -turnInput : turnInput;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, adjustedTurnInput * baseTurnSpeed * dynamicInputFactor * Time.fixedDeltaTime, 0f));
        }
    }

    private void HandleBoostInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveInput > 0.01f && cooldownTimer <= 0 && !Input.GetKey(KeyCode.Space))
        {
            isBoosting = true;
            boostTimer = boostDuration;
            cooldownTimer = boostCooldown + boostDuration;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || boostTimer <= 0)
        {
            isBoosting = false;
        }
    }
}