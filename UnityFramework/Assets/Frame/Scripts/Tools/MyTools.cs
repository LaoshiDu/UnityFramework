using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace WKC
{
    public class MyTools
    {
        /// <summary>
        /// 修改Transform的位置
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
        /// 重置Transform
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
        /// 销毁所有的子物体
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
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
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

        /// <summary>
        /// 修改物体的layer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layers"></param>
        public static void ChangeLayers(GameObject obj, string layers)
        {
            MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].gameObject.layer = LayerMask.NameToLayer(layers);
            }

            SkinnedMeshRenderer[] smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < smrs.Length; i++)
            {
                smrs[i].gameObject.layer = LayerMask.NameToLayer(layers);
            }
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepClone(object obj)
        {
            if (obj == null) return null;
            BinaryFormatter bFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bFormatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return bFormatter.Deserialize(stream);
        }
    }
}