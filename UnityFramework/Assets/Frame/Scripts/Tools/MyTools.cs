using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

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
            return UnityEngine.Random.Range(0, 1f) < probability;
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

        /// <summary>
        /// �������е�������
        /// </summary>
        /// <param name="parent"></param>
        public static void DestroyAllChild(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// ��ȡ�ļ���MD5��
        /// </summary>
        /// <param name="fileName">������ļ�������·������׺����</param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}