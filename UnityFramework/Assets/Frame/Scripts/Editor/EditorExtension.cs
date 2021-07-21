using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// �༭����չ
    /// </summary>
    public class EditorExtension
    {
        #region ���ļ���
        [MenuItem("Tools/���ļ���/Application.dataPath", false, 1)]
        private static void OpenDataPath()
        {
            OpenFolder(Application.dataPath);
        }

        [MenuItem("Tools/���ļ���/Application.persistentDataPath", false, 2)]
        private static void OpenPersistentDataPath()
        {
            OpenFolder(Application.persistentDataPath);
        }

        [MenuItem("Tools/���ļ���/Application.streamingAssetsPath", false, 3)]
        private static void OpenStreamingAssetsPath()
        {
            OpenFolder(Application.streamingAssetsPath);
        }

        /// <summary>
        /// ���ļ���
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
            Application.OpenURL("file://" + path);
        }
        #endregion

        #region �ֱ���
        [MenuItem("Tools/�ֱ���/��ȡ��Ļ��߱�", false, 21)]
        private static void GetResolution()
        {
            Debug.LogError(ResolutionDetection.GetAspectRatio());
        }

        [MenuItem("Tools/�ֱ���/�ж��Ƿ���Pad�ֱ���", false, 22)]
        private static void GetIsPad()
        {
            Debug.LogError(ResolutionDetection.IsPad());
        }
        #endregion

        #region ����UnityPackage
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
        /// ���ű�д������
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="className">����</param>
        /// <param name="baseClass">����</param>
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