using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using WKC;

public class ABEditor 
{
    private static string csvPath = Application.streamingAssetsPath + "/ABVersion.csv";

    /// <summary>
	/// 资源所在路径
	/// </summary>
	private static string _abResPath = Application.streamingAssetsPath + "/ABPackage";


    [MenuItem("Tools/生成AB包版本")]
    private static void GetABVersion()
    {
        Dictionary<string, ABVersion> abVersionsDic = new Dictionary<string, ABVersion>();
        if (File.Exists(csvPath))
        {
            string e = File.ReadAllText(csvPath);
            string[] str = e.Split('\n');
            for (int i = 1; i < str.Length; i++)
            {
                string line = str[i];
                if (line != "")
                {
                    string[] content = line.Split(',');

                    if (File.Exists(_abResPath + "/" + content[0]))
                    {
                        string newMd5 = MyTools.GetMD5HashFromFile(_abResPath + "/" + content[0]);
                        ABVersion ab;
                        content[2] = content[2].Trim();
                        if (content[2] != newMd5)
                        {
                            if (abVersionsDic.ContainsKey(content[0]))
                            {
                                abVersionsDic[content[0]].Version = Convert.ToInt32(content[1]) + 1;
                                abVersionsDic[content[0]].MD5 = newMd5;
                            }
                            else
                            {
                                ab = new ABVersion
                                {
                                    AbName = content[0],
                                    Version = Convert.ToInt32(content[1]) + 1,
                                    MD5 = newMd5
                                };
                                abVersionsDic.Add(content[0], ab);
                            }
                        }
                    }
                    else
                    {
                        abVersionsDic.Remove(content[0]);
                    }
                }
            }
            MatchFiles(abVersionsDic);
        }
        else
        {
            CreateCSV(abVersionsDic);
        }
    }

    /// <summary>
	/// 写入CSV的标题栏
	/// </summary>
	static string _dataHeard = "AbName,Version,MD5";
    /// <summary>
	/// 写入CSV的值
	/// </summary>
	static string _dataHeardValue = "{0},{1},{2}";

    private static void ResponseExportCSV(List<ABVersion> abVersions, string fileName)
    {
        if (fileName.Length > 0)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            sw.WriteLine(_dataHeard);
            //写入数据
            for (int i = 0; i < abVersions.Count; i++)
            {
                string dataStr = string.Format(_dataHeardValue, abVersions[i].AbName, abVersions[i].Version, abVersions[i].MD5);
                sw.WriteLine(dataStr);
            }
            sw.Close();
            fs.Close();
        }
        AssetDatabase.Refresh();
    }

    private static void CreateCSV(Dictionary<string, ABVersion> abVersionsDic)
    {
        if (!Directory.Exists(_abResPath))
        {
            Directory.CreateDirectory(_abResPath);
        }
        string[] files = Directory.GetFiles(_abResPath);
        foreach (string file in files)
        {
            string suffix = file.Substring(file.Length - 4);
            if (suffix != "meta")
            {
                string md5 = MyTools.GetMD5HashFromFile(file);
                string abName = file.Substring(_abResPath.Length + 1);
                ABVersion ab = new ABVersion
                {
                    AbName = abName,
                    Version = 1,
                    MD5 = md5
                };
                abVersionsDic.Add(abName, ab);
            }
        }

        List<ABVersion> abVersionsList = new List<ABVersion>();
        foreach (var item in abVersionsDic)
        {
            abVersionsList.Add(item.Value);
        }
        ResponseExportCSV(abVersionsList, csvPath);
    }

    private static void MatchFiles(Dictionary<string,ABVersion> abVersionsDic)
    {
        string[] files = Directory.GetFiles(_abResPath);
        foreach (string file in files)
        {
            string suffix = file.Substring(file.Length - 4);
            if (suffix != "meta")
            {
                string md5 = MyTools.GetMD5HashFromFile(file);
                string abName = file.Substring(_abResPath.Length + 1);
                if (!abVersionsDic.ContainsKey(abName))
                {
                    ABVersion ab = new ABVersion
                    {
                        AbName = abName,
                        Version = 1,
                        MD5 = md5
                    };
                    abVersionsDic.Add(abName, ab);
                }
            }
        }

        List<ABVersion> abVersionsList = new List<ABVersion>();
        foreach (var item in abVersionsDic)
        {
            abVersionsList.Add(item.Value);
        }
        ResponseExportCSV(abVersionsList, csvPath);
    }
}

public class ABVersion
{
    public string AbName;
    public int Version;
    public string MD5;
}