using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 编辑器扩展
    /// </summary>
    public class EditorExtension
    {
        #region 打开文件夹
        [MenuItem("Tools/打开文件夹/Application.dataPath", false, 1)]
        private static void OpenDataPath()
        {
            OpenFolder(Application.dataPath);
        }

        [MenuItem("Tools/打开文件夹/Application.persistentDataPath", false, 2)]
        private static void OpenPersistentDataPath()
        {
            OpenFolder(Application.persistentDataPath);
        }

        [MenuItem("Tools/打开文件夹/Application.streamingAssetsPath", false, 3)]
        private static void OpenStreamingAssetsPath()
        {
            OpenFolder(Application.streamingAssetsPath);
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
            Application.OpenURL("file://" + path);
        }
        #endregion

        #region 分辨率
        [MenuItem("Tools/分辨率/获取屏幕宽高比", false, 21)]
        private static void GetResolution()
        {
            Debug.LogError(ResolutionDetection.GetAspectRatio());
        }

        [MenuItem("Tools/分辨率/判断是否是Pad分辨率", false, 22)]
        private static void GetIsPad()
        {
            Debug.LogError(ResolutionDetection.IsPad());
        }
        #endregion

        #region 导出UnityPackage
        [MenuItem("Assets/Export UnityPackage %E", false, 20)]
        private static void Export()
        {
            string pathName = "Assets";
            string packageName = "Modules_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".unitypackage";
            AssetDatabase.ExportPackage(pathName, packageName, ExportPackageOptions.Recurse);
            OpenFolder(Path.Combine(Application.dataPath, "../"));
        }
        #endregion

        /// <summary>
        /// 给脚本写入内容
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="className">类名</param>
        /// <param name="baseClass">父类</param>
        public static void WriteScript(StreamWriter sw, string className, string baseClass = "MonoBehaviour")
        {
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine("using WKC;");
            sw.WriteLine();
            sw.WriteLine("public class {0} : {1}", className, baseClass);
            sw.WriteLine("{");
            sw.Write("}");
        }
    }
}