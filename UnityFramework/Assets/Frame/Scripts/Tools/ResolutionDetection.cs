using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 分辨率检测
    /// </summary>
    public class ResolutionDetection
    {
        /// <summary>
        /// 获取屏幕宽高比
        /// </summary>
        /// <returns></returns>
        public static float GetAspectRatio()
        {
            return Screen.width > Screen.height ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// 判断是否是Pad分辨率
        /// </summary>
        /// <returns></returns>
        public static bool IsPad()
        {
            return InAspectRange(4.0f / 3);
        }

        /// <summary>
        /// 判断是否是手机分辨率（大部分手机16 ：9）
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone()
        {
            return InAspectRange(16.0f / 9);
        }

        /// <summary>
        /// 判断是否是手机分辨率 3 ：2（iPhone 4s）
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone15()
        {
            return InAspectRange(3.0f / 2);
        }

        /// <summary>
        /// 判断是否是iPhoneX分辨率  2436 ：1125
        /// </summary>
        /// <returns></returns>
        public static bool IsIPhoneX()
        {
            return InAspectRange(2436.0f / 1125);
        }

        /// <summary>
        /// 分辨率的范围
        /// </summary>
        /// <param name="dstAspectRatio"></param>
        /// <returns></returns>
        private static bool InAspectRange(float dstAspectRatio)
        {
            float ratio = GetAspectRatio();
            return ratio > (dstAspectRatio - 0.05f) && ratio < (dstAspectRatio + 0.05f);
        }
    }
}