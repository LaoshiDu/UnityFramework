using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class ObjectPoolConfig
{
	/// <summary>
	/// 预制件ID
	/// </summary>
	public int ID { get; set; }

	/// <summary>
	/// AB包资源名称#分割
	/// </summary>
	public string ABName { get; set; }

	/// <summary>
	/// Resource资源路径
	/// </summary>
	public string resPath { get; set; }

	/// <summary>
	/// 初始数量
	/// </summary>
	public int PreloadAmount { get; set; }

	public static List<ObjectPoolConfig> GetList()
	{
		List<ObjectPoolConfig> list = new List<ObjectPoolConfig>();
		string csvPath = Application.persistentDataPath +"/CSVConfig/ObjectPoolConfig.csv";
		string content = File.ReadAllText(csvPath);
		string[] datas = content.Split('\n');
		for (int i = 3; i < datas.Length; i++)
		{
			string[] result = datas[i].Split(',');
			if (string.IsNullOrEmpty(result[0])) continue;
			ObjectPoolConfig config = new ObjectPoolConfig();
			for (int j = 0; j < result.Length; j++)
			{
				config.ID = int.Parse(result[0]);
				config.ABName = result[1];
				config.resPath = result[2];
				config.PreloadAmount = int.Parse(result[3]);
			}
			list.Add(config);
		}
		return list;
	}
	public static Dictionary<int,ObjectPoolConfig> GetDic()
	{
		Dictionary<int,ObjectPoolConfig> dic = new Dictionary<int,ObjectPoolConfig>();
		string csvPath = Application.persistentDataPath +"/CSVConfig/ObjectPoolConfig.csv";
		string content = File.ReadAllText(csvPath);
		string[] datas = content.Split('\n');
		for (int i = 3; i < datas.Length; i++)
		{
			string[] result = datas[i].Split(',');
			if (string.IsNullOrEmpty(result[0])) continue;
			ObjectPoolConfig config = new ObjectPoolConfig();
			for (int j = 0; j < result.Length; j++)
			{
				config.ID = int.Parse(result[0]);
				config.ABName = result[1];
				config.resPath = result[2];
				config.PreloadAmount = int.Parse(result[3]);
			}
			dic.Add(config.ID,config);
		}
		return dic;
	}
}