using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseCameraManager : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("在 Inspector 中将你的 Camera0 拖到这里")]
    [SerializeField] private Camera reverseCamera;
    [Tooltip("用于切换视角的按键，默认为 DownArrow (下箭头)")]
    [SerializeField] private KeyCode reverseKey = KeyCode.DownArrow;

    void Start()
    {
        // 确保初始状态下倒车摄像机是关闭的
        if (reverseCamera != null)
        {
            reverseCamera.enabled = false;
        }
    }

    void Update()
    {
        if (reverseCamera == null) return;

        // 检测按键按下
        if (Input.GetKey(reverseKey))
        {
            if (!reverseCamera.enabled)
            {
                reverseCamera.enabled = true;
            }
        }
        // 检测按键松开
        else if (Input.GetKeyUp(reverseKey))
        {
            reverseCamera.enabled = false;
        }
    }
}