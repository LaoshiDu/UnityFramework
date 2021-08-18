using Aspose.Cells;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 编辑器扩展
    /// </summary>
    public class EditorExtension : EditorWindow
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

        #region Excel转换
        [MenuItem("Tools/Excel转换/转换CSV")]
        private static void ToCSV()
        {
            string excelPath = Application.dataPath + "/ExcelConfig";
            string csvPath = Application.streamingAssetsPath + "/CSVConfig/";

            if (!Directory.Exists(csvPath))
            {
                Directory.CreateDirectory(csvPath);
            }

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
                        sw.Write(temp[i] + "\n");
                    }
                    sw.Close();
                    fs.Close();
                }
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Excel转换/转换Json")]
        private static void ToJson()
        {
            string csvPath = Application.streamingAssetsPath + "/CSVConfig";
            
            string[] files = Directory.GetFiles(csvPath);
            foreach (string file in files)
            {
                string suffix = file.Substring(file.Length - 4);
                if (suffix != "meta")
                {
                    string csvName = file.Substring(csvPath.Length + 1);
                    string className = csvName.Substring(0, csvName.Length - 4);

                    string content = File.ReadAllText(file);
                    string[] allContent = content.Split('\n');
                    string jsonPath = Application.dataPath + "/Resources/Configs";
                    if (!Directory.Exists(jsonPath))
                    {
                        Directory.CreateDirectory(jsonPath);
                    }
                    string fileName = jsonPath + "/" + className + ".json";

                    FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

                    string head = allContent[1];
                    string[] heads = head.Split(',');
                    string typeLine = allContent[2];
                    string[] types = typeLine.Split(',');
                    sw.WriteLine("[");

                    for (int i = 3; i < allContent.Length - 1; i++)
                    {
                        string line = allContent[i];
                        string[] txt = line.Split(',');
                        if (string.IsNullOrWhiteSpace(txt[0]) || string.IsNullOrEmpty(txt[0]))
                        {
                            continue;
                        }
                        if (i > 3)
                        {
                            sw.WriteLine("\t},");
                        }
                        sw.WriteLine("\t{");

                        for (int j = 0; j < txt.Length; j++)
                        {
                            if (types[j].Trim() == "int" || types[j].Trim() == "float")
                            {
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j].Trim() + "\"" + ":" + txt[j].Trim());
                                }
                                else
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":" + txt[j] + ",");
                                }
                            }
                            else if (types[j].Trim() == "string")
                            {
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j].Trim() + "\"" + ":" + "\"" + txt[j].Trim() + "\"");
                                }
                                else
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":" + "\"" + txt[j] + "\"" + ",");
                                }
                            }
                            else if (types[j].Trim() == "List<int>" || types[j].Trim() == "List<float>")
                            {
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j].Trim() + "\"" + ":");
                                }
                                else
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":");
                                }
                                string[] list = txt[j].Split(';');
                                sw.WriteLine("\t\t[");
                                for (int k = 0; k < list.Length; k++)
                                {
                                    if (k == list.Length - 1)
                                    {
                                        sw.WriteLine("\t\t\t" + list[k].Trim());
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t\t\t" + list[k].Trim() + ",");
                                    }
                                }
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t]");
                                }
                                else
                                {
                                    sw.WriteLine("\t\t],");
                                }
                            }
                            else if (types[j].Trim() == "List<string>")
                            {
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j].Trim() + "\"" + ":");
                                }
                                else
                                {
                                    sw.WriteLine("\t\t" + "\"" + heads[j] + "\"" + ":");
                                }
                                string[] list = txt[j].Split(';');
                                sw.WriteLine("\t\t[");
                                for (int k = 0; k < list.Length; k++)
                                {
                                    if (k == list.Length - 1)
                                    {
                                        sw.WriteLine("\t\t\t" + "\"" + list[k].Trim() + "\"");
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t\t\t" + "\"" + list[k].Trim() + "\"" + ",");
                                    }
                                }
                                if (j == txt.Length - 1)
                                {
                                    sw.WriteLine("\t\t]");
                                }
                                else
                                {
                                    sw.WriteLine("\t\t],");
                                }
                            }
                            else if (types[j].Trim() == "Dic<int,int>")
                            {

                            }
                        }
                        if (string.IsNullOrEmpty(allContent[i + 1].Trim().Split(',')[0]))
                        {
                            sw.WriteLine("\t}");
                        }
                    }
                    sw.Write("]");
                    sw.Close();
                    fs.Close();
                }
            }
            AssetDatabase.Refresh();
        }

        
        [MenuItem("Tools/Excel转换/转换C#代码")]
        private static void ToCSharp()
        {
            string csvPath = Application.streamingAssetsPath + "/CSVConfig";

            string[] files = Directory.GetFiles(csvPath);
            foreach (string file in files)
            {
                string suffix = file.Substring(file.Length - 4);
                if (suffix != "meta")
                {
                    string csvName = file.Substring(csvPath.Length + 1);
                    string className = csvName.Substring(0, csvName.Length - 4);
                    string path = Application.dataPath + "/Scripts/CSVConfigs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = path + "/" + className + ".cs";

                    string content = File.ReadAllText(file);
                    string[] temp = content.Split('\n');

                    FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.IO;");
                    sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("public class " + className);
                    sw.WriteLine("{");

                    string[] annotation = temp[0].Split(',');

                    string[] fields = temp[1].Split(',');
                    string[] fieldTyps = temp[2].Split(',');

                    for (int i = 0; i < fieldTyps.Length; i++)
                    {
                        if (string.IsNullOrEmpty(fields[i].Trim())) continue;
                        sw.WriteLine("\t/// <summary>");
                        sw.WriteLine("\t/// " + annotation[i].Trim());
                        sw.WriteLine("\t/// </summary>");
                        sw.WriteLine("\tpublic " + fieldTyps[i].Trim().Replace(';', ',') + " " + fields[i].Trim() + " { get; set; }");
                        sw.WriteLine();
                    }

                    #region List
                    sw.WriteLine("\tpublic static List<" + className + "> GetList()");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tList<" + className + "> list = new List<" + className + ">();");
                    sw.WriteLine("\t\tstring csvPath = Application.streamingAssetsPath +" + "\"" + "/CSVConfig/" + className + ".csv\";");
                    sw.WriteLine("\t\tstring content = File.ReadAllText(csvPath);");
                    sw.WriteLine("\t\tstring[] datas = content.Split('\\n');");
                    sw.WriteLine("\t\tfor (int i = 3; i < datas.Length; i++)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tstring[] result = datas[i].Split(',');");
                    sw.WriteLine("\t\t\tif (string.IsNullOrEmpty(result[0])) continue;");
                    sw.WriteLine("\t\t\t" + className + " config = new " + className + "();");
                    sw.WriteLine("\t\t\tfor (int j = 0; j < result.Length; j++)");
                    sw.WriteLine("\t\t\t{");
                    
                    for (int i = 0; i < fieldTyps.Length; i++)
                    {
                        if (fieldTyps[i].Trim() == "int")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = int.Parse(result[" + i + "]);");
                        }
                        else if (fieldTyps[i].Trim() == "float")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = float.Parse(result[" + i + "]);");
                        }
                        else if (fieldTyps[i].Trim() == "string")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = result[" + i + "];");
                        }
                        else if (fieldTyps[i].Trim() == "List<int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "s[k]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "List<float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(float.Parse(" + fields[i].Trim() + "s[k]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "List<string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "s[k]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';',',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] "+ fields[i].Trim() + "Items = "+ fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0]),int.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0]),float.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0])," + fields[i].Trim() + "Items[1]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0],int.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0],float.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0]," + fields[i].Trim() + "Items[1]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                    }

                    
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tlist.Add(config);");
                    sw.WriteLine("\t\t}");

                    sw.WriteLine("\t\treturn list;");
                    sw.WriteLine("\t}");
                    #endregion

                    #region 字典
                    sw.WriteLine("\tpublic static Dictionary<int," + className + "> GetDic()");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tDictionary<int," + className + "> dic = new Dictionary<int," + className + ">();");
                    sw.WriteLine("\t\tstring csvPath = Application.streamingAssetsPath +" + "\"" + "/CSVConfig/" + className + ".csv\";");
                    sw.WriteLine("\t\tstring content = File.ReadAllText(csvPath);");
                    sw.WriteLine("\t\tstring[] datas = content.Split('\\n');");
                    sw.WriteLine("\t\tfor (int i = 3; i < datas.Length; i++)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tstring[] result = datas[i].Split(',');");
                    sw.WriteLine("\t\t\tif (string.IsNullOrEmpty(result[0])) continue;");
                    sw.WriteLine("\t\t\t" + className + " config = new " + className + "();");
                    sw.WriteLine("\t\t\tfor (int j = 0; j < result.Length; j++)");
                    sw.WriteLine("\t\t\t{");

                    for (int i = 0; i < fieldTyps.Length; i++)
                    {
                        if (fieldTyps[i].Trim() == "int")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = int.Parse(result[" + i + "]);");
                        }
                        else if (fieldTyps[i].Trim() == "float")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = float.Parse(result[" + i + "]);");
                        }
                        else if (fieldTyps[i].Trim() == "string")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = result[" + i + "];");
                        }
                        else if (fieldTyps[i].Trim() == "List<int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "s[k]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "List<float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(float.Parse(" + fields[i].Trim() + "s[k]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "List<string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim() + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "s[k]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0]),int.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0]),float.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<int;string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(int.Parse(" + fields[i].Trim() + "Items[0])," + fields[i].Trim() + "Items[1]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;int>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0],int.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;float>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0],float.Parse(" + fields[i].Trim() + "Items[1]));");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                        else if (fieldTyps[i].Trim() == "Dictionary<string;string>")
                        {
                            sw.WriteLine("\t\t\t\tconfig." + fields[i].Trim() + " = new " + fieldTyps[i].Trim().Replace(';', ',') + "();");
                            sw.WriteLine("\t\t\t\tif (!string.IsNullOrEmpty(result[" + i + "]))");
                            sw.WriteLine("\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\tstring[] " + fields[i].Trim() + "s = result[" + i + "].Split(';');");
                            sw.WriteLine("\t\t\t\t\tfor (int k = 0; k < " + fields[i].Trim() + "s.Length; k++)");
                            sw.WriteLine("\t\t\t\t\t{");
                            sw.WriteLine("\t\t\t\t\t\tstring[] " + fields[i].Trim() + "Items = " + fields[i].Trim() + "s[k].Split('|');");
                            sw.WriteLine("\t\t\t\t\t\tconfig." + fields[i].Trim() + ".Add(" + fields[i].Trim() + "Items[0]," + fields[i].Trim() + "Items[1]);");
                            sw.WriteLine("\t\t\t\t\t}");
                            sw.WriteLine("\t\t\t\t}");
                        }
                    }


                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tdic.Add(config.ID,config);");
                    sw.WriteLine("\t\t}");

                    sw.WriteLine("\t\treturn dic;");
                    sw.WriteLine("\t}");
                    #endregion

                    sw.Write("}");
                    sw.Close();
                    fs.Close();
                }
            }
            AssetDatabase.Refresh();
        }
        #endregion

        #region 清除数据
        [MenuItem("Tools/清除数据/清除PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        [MenuItem("Tools/清除数据/清除Json")]
        private static void ClearJson()
        {
            if (Directory.Exists(Application.persistentDataPath + "/UserData"))
            {
                Directory.Delete(Application.persistentDataPath + "/UserData", true);
            }
        }
        #endregion
    }
}