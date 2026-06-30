using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelData : MonoBehaviour
{
    [Header("摩擦与抓地力")]
    public float longitudinalFriction = 0.8f; // 纵向摩擦（加速/刹车）
    public float lateralFriction = 0.6f;     // 横向摩擦（转向/漂移）

    [Header("轮子几何参数")]
    public float wheelRadius = 0.35f;        // 轮子半径
    public bool isPowered = false;           // 是否为驱动轮
    public bool isSteerable = false;         // 是否为转向轮

    [Header("动态数据")]
    public float slipAmount;                 // 当前滑动量
    public float currentTorque;              // 当前施加的扭矩
    public float steerAngle;                 // 当前转向角度
}