using LitJson;
using System.IO;
using UnityEngine;

namespace WKC
{
    public class DataStorage : BaseMgr<DataStorage>
    {
        public UserData userdata;

        private void InitData()
        {
            userdata = new UserData();
            userdata.version = GameConfig.Instance.version;
            userdata.gold = 0;
            SaveJsonDataToLocal();
        }

        private FileInfo _saveLocalFileInfo = new FileInfo(Application.persistentDataPath + @"/UserData/UserData.json");

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public void SaveJsonDataToLocal()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/UserData"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/UserData");
            }
            using (StreamWriter sw = _saveLocalFileInfo.CreateText())
            {
                var result = JsonMapper.ToJson(userdata);
                sw.Write(result);
            }
        }

        /// <summary>
        /// 加载玩家数据
        /// </summary>
        public void LoadJsonsData()
        {
            if (GameConfig.Instance.isClearUserData)
            {
                if (_saveLocalFileInfo.Exists)
                {
                    File.Delete(Application.persistentDataPath + @"/UserData/UserData.json");
                }
                InitData();
                return;
            }

            if (!_saveLocalFileInfo.Exists)
            {
                InitData();
            }
            else
            {
                string path = Application.persistentDataPath + @"/UserData/UserData.json";

                StreamReader reader = new StreamReader(path);
                userdata = JsonMapper.ToObject<UserData>(reader);
                reader.Close();
                if (userdata == null || GameConfig.Instance.version != userdata.version)
                {
                    InitData();
                }
            }
        }
    }
}