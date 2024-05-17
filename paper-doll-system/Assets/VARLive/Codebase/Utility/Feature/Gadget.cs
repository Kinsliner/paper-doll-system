using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Tool
{
    public static class Gadget
    {

        // 用於隨機偏移向量的函數
        /// <summary>
        /// 用於隨機偏移向量的函數
        /// </summary>
        /// <param name="direction">原始向量</param>
        /// <param name="angle">偏移角度量</param>
        /// <param name="mask">決定是否偏移向量的遮罩</param>
        /// <returns></returns>
        public static Vector3 RandomizeVector(Vector3 direction, float angle, Vector3 mask)
        {
            // 隨機生成一個圍繞 y 軸的旋轉角度
            float randomAngle = Random.Range(-angle, angle);
            // 產生遮罩
            mask = mask.ToMask();
            // 旋轉方向
            Vector3 euler = randomAngle.ToVector3(mask);
            // 將旋轉角度轉換成 Quaternion
            Quaternion randomRotation = Quaternion.Euler(euler);
            // 將原始方向向量應用旋轉
            Vector3 randomOffset = randomRotation * direction;
            return randomOffset.normalized; // 正規化向量
        }
    }
}