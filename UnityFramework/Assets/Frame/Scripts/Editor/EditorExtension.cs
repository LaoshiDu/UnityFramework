using Aspose.Cells;
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

        #region Excelת��
        [MenuItem("Tools/Excelת��/ת��CSV")]
        private static void ToCSV()
        {
            string excelPath = Application.dataPath + "/ExcelConfig";
            string csvPath = Application.dataPath + "/CSVConfig/";

            string[] files = Directory.GetFiles(excelPath);
            foreach (string file in files)
            {
                string suffix = file.Substring(file.Length - 4);
                if (suffix != "meta")
                {
                    Workbook wb = new Workbook(file);
                    string excelName = file.Substring(excelPath.Length + 1);
                    string path = csvPath + excelName.Substring(0, excelName.Length - 5) + ".csv";
                    wb.Save(path, SaveFormat.CSV);
                    string content = File.ReadAllText(path);
                    string[] temp = content.Split('\n');
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    for (int i = 0; i < temp.Length - 1; i++)
                    {
                        sw.Write(temp[i]+"\n");
                    }
                    sw.Close();
                    fs.Close();
                }
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Excelת��/ת��Json")]
        private static void ToJson()
        {
            string csvPath = Application.dataPath + "/CSVConfig/ActorConfig.csv";
            string content = File.ReadAllText(csvPath);
            string[] allContent = content.Split('\n');
            string jsonPath = Application.dataPath + "/Resources/Configs";
            if (!Directory.Exists(jsonPath))
            {
                Directory.CreateDirectory(jsonPath);
            }

            string fileName = jsonPath + "/ActorConfig.json";

            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            string head = allContent[1];
            string[] heads = head.Split(',');

            string typeLine = allContent[2];
            string[] types = typeLine.Split(',');
            sw.WriteLine("[\n");
            for (int i = 3; i < allContent.Length; i++)
            {
                sw.WriteLine("\t{\n");
                string line = allContent[i];
                string[] txt = line.Split(',');

                for (int j = 0; j < txt.Length; j++)
                {
                    if (j < types.Length)
                    {
                        if (types[j] == "int" || types[j] == "float")
                        {
                            sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":" + txt[j]);
                        }
                        else if (types[j] == "string")
                        {
                            sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":" + "\"" + txt[j] + "\"");
                        }
                        else if (types[j] == "List<int>")
                        {
                            //string[] list = txt[j].Split(';');
                            //sw.WriteLine("\t\t[");
                            //for (int k = 0; k < list.Length; k++)
                            //{

                            //}
                            //sw.WriteLine("\t\t]");
                        }
                        else if (types[j]=="Dic<,>")
                        {
                        }
                    }
                }
                if (i == allContent.Length - 1)
                {
                    sw.WriteLine("\t}");
                }
                else
                {
                    sw.WriteLine("\t},");
                }
            }
            sw.Write("]");
            sw.Close();
            fs.Close();
            AssetDatabase.Refresh();
        }
        #endregion
    }
}