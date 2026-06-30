using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    [Header("相机引用")]
    [SerializeField] private GameObject firstPersonCam; // Camera1
    [SerializeField] private GameObject thirdPersonCam; // 你的第三人称相机

    private bool isThirdPerson = false;

    void Start()
    {
        // 初始设置
        UpdateCameraState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isThirdPerson = !isThirdPerson;
            UpdateCameraState();
        }
    }

    private void UpdateCameraState()
    {
        firstPersonCam.SetActive(!isThirdPerson);
        thirdPersonCam.SetActive(isThirdPerson);

        // 确保无论切换到哪个视角，鼠标都是锁定的
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}