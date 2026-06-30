using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionController : MonoBehaviour
{
    [Header("设置")]
    public Transform[] wheelPositions; // 将四个轮子的 Transform 拖入这里
    public float suspensionHeight = 0.5f; // 车身离地高度
    public LayerMask groundLayer;      // 地面所在的图层

    void Update()
    {
        float totalHeight = 0;
        int groundedWheels = 0;

        foreach (Transform wheel in wheelPositions)
        {
            RaycastHit hit;
            // 从轮子位置向下发射射线
            if (Physics.Raycast(wheel.position, Vector3.down, out hit, 1.0f, groundLayer))
            {
                // 计算当前轮子离地高度并累加
                totalHeight += hit.point.y;
                groundedWheels++;
            }
        }

        if (groundedWheels > 0)
        {
            // 计算车身目标高度
            float averageY = (totalHeight / groundedWheels) + suspensionHeight;

            // 线性平滑移动车身 (你可以根据需要调整插值速度)
            Vector3 targetPos = new Vector3(transform.position.x, averageY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
        }
    }
}