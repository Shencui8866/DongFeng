using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing
{
    public class VectorExtension
    {
        /// <summary>
        /// 获取向量角度
        /// </summary>
        /// <param name="v1">当前朝向</param>
        /// <param name="oldAngle">当前标记角度</param>
        /// <param name="limitDeltaDiscard">角度阈值，超过丢弃</param>
        /// <returns></returns>
        public static float DeltaAngle(Vector2 v1, ref float oldAngle, float limitDeltaDiscard)
        {
            float delta = 0;
            float sign = 0;
            float directAngle = 0;
            float deltaNewAngle = 0;
            directAngle = Vector2.Angle(v1, Vector2.left);
            sign = Mathf.Sign(Vector2.Dot(v1, Vector2.up));
            if (sign < 0) deltaNewAngle = 360 + directAngle * sign;
            else deltaNewAngle = directAngle;

            delta = oldAngle - deltaNewAngle;
            oldAngle = deltaNewAngle;

            if (Mathf.Abs(delta) > limitDeltaDiscard) delta = 0;
            return delta;
        }
    }
}


