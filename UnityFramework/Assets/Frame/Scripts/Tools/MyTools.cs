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

        /// <summary>
        /// 世界坐标转UI坐标
        /// </summary>
        /// <param name="targetWorldPos"></param>
        /// <param name="mainCam"></param>
        /// <param name="uiCam"></param>
        /// <param name="uiCanvas"></param>
        /// <returns></returns>
        public static Vector3 World2UIPos(Vector3 targetWorldPos, Camera mainCam, Camera uiCam, RectTransform uiCanvas)
        {
            Vector3 zero = Vector3.zero;
            Vector3 vector = mainCam.WorldToScreenPoint(targetWorldPos);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiCanvas, vector, uiCam, out zero);
            return zero;
        }
        /// <summary>
        /// UI坐标转世界坐标
        /// </summary>
        /// <param name="uiTarget"></param>
        /// <returns></returns>
        public static Vector3 UI2WorldPos(RectTransform uiTarget)
        {
            CanvasScaler canvasScaler = UIManager.Instance.canvasScaler;
            Vector2 referenceResolution = canvasScaler.referenceResolution;
            Vector2 vector = uiTarget.anchoredPosition;
            CanvasScaler.ScreenMatchMode screenMatchMode = canvasScaler.screenMatchMode;
            if (screenMatchMode == CanvasScaler.ScreenMatchMode.Expand||screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight|| screenMatchMode== CanvasScaler.ScreenMatchMode.Shrink)
            {
                if ((int)screenMatchMode - 1 <= 1)
                {
                    float num = (float)Screen.width / referenceResolution.x;
                    float num2 = (float)Screen.height / referenceResolution.y;
                    vector.y *= num2;
                    vector.x *= num;
                }
            }
            else
            {
                float num3 = (canvasScaler.matchWidthOrHeight == 0f) ? ((float)Screen.width / referenceResolution.x) : ((float)Screen.height / referenceResolution.y);
                vector *= num3;
            }
            Vector2 vector2 = vector + 0.5f * new Vector2((float)Screen.width, (float)Screen.height);
            Vector3 result = default(Vector3);
            UIManager instance = UIManager.Instance;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(instance.root, vector2, CameraManager.Instance.uiCamera, out result);
            return result;
        }

        /// <summary>
        /// 屏幕坐标转3D本地坐标
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="parent"></param>
        /// <param name="mainCam"></param>
        /// <returns></returns>
        public static Vector3 ScreenToLocalPos(Vector3 screenPos, Transform parent, Camera mainCam)
        {
            Vector3 vector= new Vector3(screenPos.x, screenPos.y, -mainCam.transform.position.z);
            Vector3 vector2 = mainCam.ScreenToWorldPoint(vector);
            return parent.worldToLocalMatrix.MultiplyPoint(vector2);
        }

        /// <summary>
        /// 屏幕坐标向指定Canvas画布坐标转换(Local)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="screen"></param>
        /// <param name="uiCam"></param>
        /// <returns></returns>
        public static Vector2 ScreenToCanvasPos(Canvas canvas, Vector3 screen, Camera uiCam)
        {
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screen, uiCam, out result);
            return result;
        }
    }
}