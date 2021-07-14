using UnityEngine;

namespace WKC
{
    /// <summary>
    /// �ֱ��ʼ��
    /// </summary>
    public class ResolutionDetection
    {
        /// <summary>
        /// ��ȡ��Ļ��߱�
        /// </summary>
        /// <returns></returns>
        public static float GetAspectRatio()
        {
            return Screen.width > Screen.height ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// �ж��Ƿ���Pad�ֱ���
        /// </summary>
        /// <returns></returns>
        public static bool IsPad()
        {
            return InAspectRange(4.0f / 3);
        }

        /// <summary>
        /// �ж��Ƿ����ֻ��ֱ��ʣ��󲿷��ֻ�16 ��9��
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone()
        {
            return InAspectRange(16.0f / 9);
        }

        /// <summary>
        /// �ж��Ƿ����ֻ��ֱ��� 3 ��2��iPhone 4s��
        /// </summary>
        /// <returns></returns>
        public static bool IsPhone15()
        {
            return InAspectRange(3.0f / 2);
        }

        /// <summary>
        /// �ж��Ƿ���iPhoneX�ֱ���  2436 ��1125
        /// </summary>
        /// <returns></returns>
        public static bool IsIPhoneX()
        {
            return InAspectRange(2436.0f / 1125);
        }

        /// <summary>
        /// �ֱ��ʵķ�Χ
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