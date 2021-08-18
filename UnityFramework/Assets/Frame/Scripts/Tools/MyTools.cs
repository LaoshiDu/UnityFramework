using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WKC
{
    public class MyTools
    {
        /// <summary>
        /// ���ݸ��ʻ�ȡ���
        /// </summary>
        /// <param name="probability">����ֵ 0-1 </param>
        /// <returns></returns>
        public static bool RandomResult(float probability)
        {
            return Random.Range(0, 1f) < probability;
        }

        /// <summary>
        /// �޸�Transform��λ��
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 SetTransPos(Vector3 pos, float x = 0, float y = 0, float z = 0)
        {
            pos.Set(x == 0 ? pos.x : x, y == 0 ? pos.y : y, z == 0 ? pos.z : z);
            return pos;
        }

        /// <summary>
        /// ����Transform
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="parent"></param>
        public static void ResetTrans(Transform trans, Transform parent = null)
        {
            trans.SetParent(parent);
            trans.localScale = Vector3.one;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
        }
    }
}