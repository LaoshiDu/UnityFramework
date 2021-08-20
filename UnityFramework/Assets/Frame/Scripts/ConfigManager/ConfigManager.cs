using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace WKC
{
    public class ConfigManager : BaseMgr<ConfigManager>
    {
        private string csvPath = Application.persistentDataPath + "/CSVConfig/";
        private int csvCount = 0;
        private int completedCount = 0;
        private EventCall completed;

        public void Init(EventCall callback)
        {
            completed = callback;
            string streamingAssetsPath = Application.streamingAssetsPath + "/CSVConfig";
            
            if (!Directory.Exists(csvPath))
            {
                Directory.CreateDirectory(csvPath);
                
                string[] csvFileNames = Configs.configsString.Split('|');

                csvCount = csvFileNames.Length;

                for (int i = 0; i < csvCount; i++)
                {
                    string fileName = csvFileNames[i] + ".csv";
                    string configPath = Path.Combine(streamingAssetsPath, fileName);
                    TimeManager.Instance.StartCoroutine(ReadData(configPath, fileName));
                }
            }
            else
            {
                callback?.Invoke();
            }
        }

        IEnumerator ReadData(string path, string fileName)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        File.WriteAllBytes(csvPath + fileName, webRequest.downloadHandler.data);
                        completedCount++;
                        if (completedCount == csvCount)
                        {
                            completed?.Invoke();
                        }
                        break;
                }
            }
        }
    }
}