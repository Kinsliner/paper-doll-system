using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // 跟隨目標
        public float smoothSpeed = 0.125f; // 平滑度
        public Vector3 offset; // 偏移量

        // 在LateUpdate中執行，確保角色移動後才執行
        void LateUpdate()
        {
            if (target == null)
                return;

            // 計算攝影機的目標位置
            Vector3 desiredPosition = target.position + offset;

            // 使用Lerp方法平滑移動攝影機
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 設定攝影機的位置
            transform.position = smoothedPosition;
        }
    }

}
