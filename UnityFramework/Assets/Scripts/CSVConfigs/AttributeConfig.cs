using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class AttributeConfig
{
	/// <summary>
	/// ID
	/// </summary>
	public int ID { get; set; }

	/// <summary>
	/// 角色
	/// </summary>
	public int roleId { get; set; }

	/// <summary>
	/// 角色名多语言表ID
	/// </summary>
	public int name { get; set; }

	/// <summary>
	/// 等级
	/// </summary>
	public int grade { get; set; }

	/// <summary>
	/// 品阶
	/// </summary>
	public string quality { get; set; }

	/// <summary>
	/// 攻击力
	/// </summary>
	public float attack { get; set; }

	/// <summary>
	/// PVP攻击力
	/// </summary>
	public float pvpattack { get; set; }

	/// <summary>
	/// buff
	/// </summary>
	public int buff { get; set; }

	/// <summary>
	/// 血量
	/// </summary>
	public int hp { get; set; }

	/// <summary>
	/// 攻击速度
	/// </summary>
	public float aSpeed { get; set; }

	/// <summary>
	/// 移动速度
	/// </summary>
	public float mSpeed { get; set; }

	/// <summary>
	/// 子弹速度
	/// </summary>
	public float fSpeed { get; set; }

	/// <summary>
	/// 攻击范围
	/// </summary>
	public float scope { get; set; }

	/// <summary>
	/// 宠物ID
	/// </summary>
	public int pet { get; set; }

	/// <summary>
	/// 升级预览
	/// </summary>
	public string preview { get; set; }

	/// <summary>
	/// 升级消耗
	/// </summary>
	public string consume { get; set; }

	/// <summary>
	/// 掉落
	/// </summary>
	public int dropId { get; set; }

	/// <summary>
	/// 战力
	/// </summary>
	public float fight { get; set; }

	/// <summary>
	/// 锁定范围
	/// </summary>
	public float lockScope { get; set; }

	/// <summary>
	/// 免伤比例
	/// </summary>
	public float reduction { get; set; }

	public static List<AttributeConfig> GetList()
	{
		List<AttributeConfig> list = new List<AttributeConfig>();
		string csvPath = Application.persistentDataPath +"/CSVConfig/AttributeConfig.csv";
		string content = File.ReadAllText(csvPath);
		string[] datas = content.Split('\n');
		for (int i = 3; i < datas.Length; i++)
		{
			string[] result = datas[i].Split(',');
			if (string.IsNullOrEmpty(result[0])) continue;
			AttributeConfig config = new AttributeConfig();
			for (int j = 0; j < result.Length; j++)
			{
				config.ID = int.Parse(result[0]);
				config.roleId = int.Parse(result[1]);
				config.name = int.Parse(result[2]);
				config.grade = int.Parse(result[3]);
				config.quality = result[4];
				config.attack = float.Parse(result[5]);
				config.pvpattack = float.Parse(result[6]);
				config.buff = int.Parse(result[7]);
				config.hp = int.Parse(result[8]);
				config.aSpeed = float.Parse(result[9]);
				config.mSpeed = float.Parse(result[10]);
				config.fSpeed = float.Parse(result[11]);
				config.scope = float.Parse(result[12]);
				config.pet = int.Parse(result[13]);
				config.preview = result[14];
				config.consume = result[15];
				config.dropId = int.Parse(result[16]);
				config.fight = float.Parse(result[17]);
				config.lockScope = float.Parse(result[18]);
				config.reduction = float.Parse(result[19]);
			}
			list.Add(config);
		}
		return list;
	}
	public static Dictionary<int,AttributeConfig> GetDic()
	{
		Dictionary<int,AttributeConfig> dic = new Dictionary<int,AttributeConfig>();
		string csvPath = Application.persistentDataPath +"/CSVConfig/AttributeConfig.csv";
		string content = File.ReadAllText(csvPath);
		string[] datas = content.Split('\n');
		for (int i = 3; i < datas.Length; i++)
		{
			string[] result = datas[i].Split(',');
			if (string.IsNullOrEmpty(result[0])) continue;
			AttributeConfig config = new AttributeConfig();
			for (int j = 0; j < result.Length; j++)
			{
				config.ID = int.Parse(result[0]);
				config.roleId = int.Parse(result[1]);
				config.name = int.Parse(result[2]);
				config.grade = int.Parse(result[3]);
				config.quality = result[4];
				config.attack = float.Parse(result[5]);
				config.pvpattack = float.Parse(result[6]);
				config.buff = int.Parse(result[7]);
				config.hp = int.Parse(result[8]);
				config.aSpeed = float.Parse(result[9]);
				config.mSpeed = float.Parse(result[10]);
				config.fSpeed = float.Parse(result[11]);
				config.scope = float.Parse(result[12]);
				config.pet = int.Parse(result[13]);
				config.preview = result[14];
				config.consume = result[15];
				config.dropId = int.Parse(result[16]);
				config.fight = float.Parse(result[17]);
				config.lockScope = float.Parse(result[18]);
				config.reduction = float.Parse(result[19]);
			}
			dic.Add(config.ID,config);
		}
		return dic;
	}
}