using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [Header("绑定目标")]
    [SerializeField] private Transform carTarget;

    [Header("跟随与偏移设置")]
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private float turnLagIntensity = 3f;
    [SerializeField] private float smoothTurnSpeed = 4f;

    [Header("鼠标视角设置")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxViewAngle = 60f;
    [SerializeField] private float autoResetDelay = 2f;    // 自动归正延迟(秒)
    [SerializeField] private float resetSpeed = 5f;        // 归正平滑速度

    [Header("FOV 动态")]
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float maxSpeedFOV = 80f;

    [Header("视角模式")]
    [SerializeField] private bool isFirstPerson = false; // 勾选此项即为第一人称模式

    private Vector3 initialOffset;
    private float currentTurnOffset = 0f;
    private float mouseRotationY = 0f;
    private float idleTimer = 0f; // 监测鼠标是否静止
    private Camera cam;

    void Start()
    {
        initialOffset = transform.position - carTarget.position;
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (carMovement == null || carTarget == null) return;

        // 1. 处理鼠标视角输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        if (Mathf.Abs(mouseX) > 0.01f)
        {
            mouseRotationY = Mathf.Clamp(mouseRotationY + mouseX, -maxViewAngle, maxViewAngle);
            idleTimer = 0f; // 有输入时重置计时器
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        // 2. 归正逻辑：自动归正 或 按下V强制复位
        if (Input.GetKeyDown(KeyCode.V) || (idleTimer >= autoResetDelay))
        {
            mouseRotationY = Mathf.MoveTowards(mouseRotationY, 0f, Time.deltaTime * resetSpeed * 20f);
        }

        // 3. 转向延迟逻辑 (原有逻辑)
        float turnInput = Input.GetAxis("Horizontal");
        float targetOffset = -turnInput * turnLagIntensity;
        currentTurnOffset = Mathf.Lerp(currentTurnOffset, targetOffset, Time.deltaTime * smoothTurnSpeed);

        // 4. 计算跟随位置
        Quaternion rotationOffset = Quaternion.Euler(0f, mouseRotationY, 0f);
        Vector3 rotatedOffset = rotationOffset * initialOffset;

        Vector3 targetPos = carTarget.position + (carTarget.rotation * rotatedOffset);
        Vector3 finalPos = targetPos + (carTarget.right * currentTurnOffset);

        transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * followSpeed);
        transform.LookAt(carTarget.position);

        // 5. 动态 FOV 逻辑
        float speed = Mathf.Abs(carMovement.GetCurrentSpeed());
        float speedRatio = Mathf.Clamp01(speed / 180f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(normalFOV, maxSpeedFOV, speedRatio), Time.deltaTime * 5f);
    }
}