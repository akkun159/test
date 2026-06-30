using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // 确保挂载此脚本的物体拥有刚体
public class CarPhysicsSetup : MonoBehaviour
{
    // 在 Inspector 中手动设置重心下移的数值
    [Tooltip("重心距离物体底部的偏移量")]
    public float centerOfMassOffset = -0.5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 设置重心
        // centerOfMass 是相对于物体坐标系的原点(pivot)的
        rb.centerOfMass = new Vector3(0, centerOfMassOffset, 0);
    }

    // 在编辑器中可视化显示重心位置，方便调试
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 在编辑器中画一个红球，标出重心位置
        Gizmos.DrawSphere(transform.TransformPoint(new Vector3(0, centerOfMassOffset, 0)), 0.3f);
    }
}