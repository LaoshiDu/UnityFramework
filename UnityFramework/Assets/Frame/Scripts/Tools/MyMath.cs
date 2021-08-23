using System;
using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class MyMath
    {
        /// <summary>
        /// 返回向量之间夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fromZero"></param>
        /// <returns></returns>
        public static float VectorAngle(Vector2 from, Vector2 to, bool fromZero = false)
        {
            if (fromZero)
            {
                return Vector2.Angle(from, to);
            }
            Vector3 vector = Vector3.Cross(from, to);
            float num = Vector2.Angle(from, to);
            if (vector.z <= 0f)
            {
                return num;
            }
            return -num;
        }

        /// <summary>
        /// 得到权重的随机数
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public static int RandomWeightIndex(List<int> prob)
        {
            int result = 0;
            int num = 0;
            for (int i = 0; i < prob.Count; i++)
            {
                num += prob[i];
            }
            int num2 = num * 1000;
            int num3 = UnityEngine.Random.Range(0, num2);
            int num4 = 0;
            for (int j = 0; j < prob.Count; j++)
            {
                if (prob[j] != 0)
                {
                    int num5 = num4 + prob[j];
                    if (num3 >= num4 * 1000 && num3 < num5 * 1000)
                    {
                        result = j;
                        break;
                    }
                    num4 = num5;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据概率获取结果
        /// </summary>
        /// <param name="probability">概率值 0-1 </param>
        /// <returns></returns>
        public static bool RandomResult(float probability)
        {
            return UnityEngine.Random.Range(0, 1f) < probability;
        }

        /// <summary>
        /// 根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Vector3 BezierCurve(Vector3 P0, Vector3 P1, Vector3 P2, float t)
        {
            Vector3 zero = Vector3.zero;
            float num = (1f - t) * (1f - t);
            float num2 = t * (1f - t);
            float num3 = t * t;
            return P0 * num + 2f * num2 * P1 + num3 * P2;
        }

        /// <summary>
        /// 获取存储贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="controlPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="segmentNum"></param>
        /// <returns></returns>
        public static Vector3[] GetBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
        {
            Vector3[] array = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = (float)i / (float)segmentNum;
                Vector3 vector = BezierCurve(startPoint, controlPoint, endPoint, t);
                array[i - 1] = vector;
            }
            return array;
        }

        /// <summary>
        /// 点到线段最近的点的位置和距离
        /// </summary>
        /// <param name="linePt1"></param>
        /// <param name="linePt2"></param>
        /// <param name="point"></param>
        /// <param name="retPoint"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool ClosestPointOnLine(Vector2 linePt1, Vector2 linePt2, Vector2 point, out Vector2 retPoint, out float d)
        {
            Matrix4x4 matrix4x = Matrix4x4.TRS(linePt1, Quaternion.Euler(0f, 0f, CalcIncludedAngle2D(Vector2.right, linePt2 - linePt1)), Vector3.one);
            Matrix4x4 inverse = matrix4x.inverse;
            point = inverse.MultiplyPoint(point);
            Vector2 vector = inverse.MultiplyPoint(linePt2);
            bool flag = point.x > 0f != point.x > vector.x;
            if (flag)
            {
                d = Mathf.Abs(point.y);
                retPoint = matrix4x.MultiplyPoint(new Vector3(point.x, 0f, 0f));
            }
            else
            {
                float magnitude = point.magnitude;
                float magnitude2 = (point - vector).magnitude;
                d = Mathf.Min(magnitude, magnitude2);
                retPoint = ((magnitude < magnitude2) ? linePt1 : linePt2);
            }
            return flag;
        }

        /// <summary>
        /// 2D坐标下某点是否存在于多边形内
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static bool IsPointInPoly(Vector2 testPoint, Vector2[] poly)
        {
            bool flag = false;
            int i = 0;
            int num = poly.Length - 1;
            while (i < poly.Length)
            {
                if (poly[i].y > testPoint.y != poly[num].y > testPoint.y && testPoint.x > poly[i].x + (poly[num].x - poly[i].x) * (poly[i].y - testPoint.y) / (poly[i].y - poly[num].y))
                {
                    flag = !flag;
                }
                num = i++;
            }
            return flag;
        }

        /// <summary>
        /// 判断点是否在直线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float IsPointOnLine(Vector2 point, Vector2 start, Vector2 end)
        {
            return (start.x - point.x) * (end.y - point.y) - (end.x - point.x) * (start.y - point.y);
        }

        /// <summary>
        /// 判断两个向量是否平行
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsParallel(Vector3 line1, Vector3 line2)
        {
            return Mathf.Abs(Vector3.Dot(line1.normalized, line2.normalized)) == 1f;
        }

        /// <summary>
        /// 判断两个向量是否平行
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsVertical(Vector3 line1, Vector3 line2)
        {
            return Vector3.Dot(line1.normalized, line2.normalized) == 0f;
        }

        /// <summary>
        /// 判断两个直线的交点
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CalcLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 result)
        {
            if (IsParallel(p2 - p1, p4 - p3))
            {
                result = Vector2.zero;
                Debug.LogWarning(new Exception("CalcLineIntersection 两条直线平行，无交点"));
                return false;
            }
            float num = (p2.y - p1.y) * (p4.x - p3.x) - (p4.y - p3.y) * (p2.x - p1.x);
            float num2 = (p3.y - p1.y) * (p2.x - p1.x) * (p4.x - p3.x) + (p2.y - p1.y) * (p4.x - p3.x) * p1.x - (p4.y - p3.y) * (p2.x - p1.x) * p3.x;
            result.x = num2 / num;
            num = (p2.x - p1.x) * (p4.y - p3.y) - (p4.x - p3.x) * (p2.y - p1.y);
            num2 = (p3.x - p1.x) * (p2.y - p1.y) * (p4.y - p3.y) + p1.y * (p2.x - p1.x) * (p4.y - p3.y) - p3.y * (p4.x - p3.x) * (p2.y - p1.y);
            result.y = num2 / num;
            return true;
        }

        /// <summary>
        /// 计算两个三维向量的夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float CalcIncludedAngle3D(Vector3 from, Vector3 to)
        {
            from.y = from.z;
            to.y = to.z;
            Vector2 from2 = from;
            Vector2 to2 = to;
            return CalcIncludedAngle2D(from2, to2);
        }

        /// <summary>
        /// 计算两个二维向量的夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float CalcIncludedAngle2D(Vector2 from, Vector2 to)
        {
            Vector3 vector = Vector3.Cross(from, to);
            if (vector.z <= 0f)
            {
                return -Vector2.Angle(from, to);
            }
            return Vector2.Angle(from, to);
        }

        /// <summary>
        /// 获取GameObject的包围盒
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Bounds CalcGamaObjectBoundsInWorld(GameObject go)
        {
            Transform transform = go.transform;
            Vector3 vector = Vector3.one * float.MinValue;
            Vector3 vector2 = Vector3.one * float.MaxValue;
            Bounds result = default(Bounds);
            Vector3[] array = new Vector3[8];
            if (!go.activeInHierarchy)
            {
                return result;
            }
            MeshFilter component = go.GetComponent<MeshFilter>();
            if (component && component.sharedMesh)
            {
                component.sharedMesh.RecalculateBounds();
                result = component.sharedMesh.bounds;
            }
            else
            {
                SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
                if (component2 && component2.rootBone)
                {
                    transform = component2.rootBone;
                    result = component2.localBounds;
                }
            }
            Vector3 extents = result.extents;
            array[0] = result.center + new Vector3(-extents.x, extents.y, extents.z);
            array[1] = result.center + new Vector3(-extents.x, extents.y, -extents.z);
            array[2] = result.center + new Vector3(extents.x, extents.y, extents.z);
            array[3] = result.center + new Vector3(extents.x, extents.y, -extents.z);
            array[4] = result.center + new Vector3(-extents.x, -extents.y, extents.z);
            array[5] = result.center + new Vector3(-extents.x, -extents.y, -extents.z);
            array[6] = result.center + new Vector3(extents.x, -extents.y, extents.z);
            array[7] = result.center + new Vector3(extents.x, -extents.y, -extents.z);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = transform.localToWorldMatrix.MultiplyPoint(array[i]);
            }
            for (int j = 0; j < array.Length; j++)
            {
                vector2.x = Mathf.Min(array[j].x, vector2.x);
                vector2.y = Mathf.Min(array[j].y, vector2.y);
                vector2.z = Mathf.Min(array[j].z, vector2.z);
                vector.x = Mathf.Max(array[j].x, vector.x);
                vector.y = Mathf.Max(array[j].y, vector.y);
                vector.z = Mathf.Max(array[j].z, vector.z);
            }
            result.SetMinMax(vector2, vector);
            for (int k = 0; k < go.transform.childCount; k++)
            {
                result.Encapsulate(CalcGamaObjectBoundsInWorld(go.transform.GetChild(k).gameObject));
            }
            return result;
        }


        /// <summary>
        /// 数字转换
        /// </summary>
        /// <param name="num"></param>
        /// <param name="b">保留小数位数</param>
        /// <returns></returns>
        public static string NumToString(float num, int t = 2)
        {
            try
            {
                //初始化数据
                List<NumFormat> numDatas = new List<NumFormat>() {
                new NumFormat() {Value=1,symbol="" },
                new NumFormat() {Value=1e3,symbol="K" },
                new NumFormat() {Value=1e6,symbol="B" },
                new NumFormat() {Value=1e9,symbol="T" },
                new NumFormat() {Value=1e12,symbol="aa" },
                new NumFormat() {Value=1e15,symbol="ab" },
                new NumFormat() {Value=1e18,symbol="ac" }};
                int i = 0;
                for (i = numDatas.Count - 1; i > 0; i--)
                {
                    if (num >= numDatas[i].Value)
                    {
                        break;
                    }
                }
                return Math.Round(num / numDatas[i].Value, t) + numDatas[i].symbol;
            }
            catch (Exception ex)
            {
                Console.WriteLine("数字转换异常：" + ex.Message);
            }
            return "数字转换异常"+ num;
        }
    }
}