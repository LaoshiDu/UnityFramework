using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace WKC
{

    /// <summary>
    /// csv资源文件内容
    /// </summary>
    public class ABVersion
    {
        public string AbName;
        public int Version;
        public string MD5;
    }

    public class AssetBundleCheckManager : BaseMgr<AssetBundleCheckManager>
    {
        private Dictionary<string, AssetBundle> _LoadedBundles = new Dictionary<string, AssetBundle>();

        private bool _isLoadAB;

        int _loadAbNum = -1;

        public void Init(EventCall completed)
        {
            switch (GameConfig.Instance.loadType)
            {
                case AssetLoadType.Resources:
                    completed?.Invoke();
                    break;
                case AssetLoadType.AssetBundle:
                    callback = completed;
                    _isLoadAB = false;
                    DownLoadVersion();
                    break;
            }
        }

        private void DownLoadVersion()
        {
            //启动游戏检测
            //下载最新的资源配置信息
            //根据本地是否存在资源配置信息
            //有就根据版本更新,没有就去下载完整ab资源
            //更新完成或者下载完成,才开始进入游戏

            //本地资源配置信息文件路径
            string ABVersionPath = Application.persistentDataPath + "/ABVersion.csv";
            //记录最新的资源配置信息,用于保存最新的csv文件
            List<ABVersion> abVersions = new List<ABVersion>();
            ABVersion ab;
            //本地存在,更新
            if (File.Exists(ABVersionPath))
            {
                //临时保存的资源配置路径,
                string TemCSVPath = Application.persistentDataPath + "/TemCSV";

                if (!Directory.Exists(TemCSVPath))
                    Directory.CreateDirectory(TemCSVPath);

                //下载服务器csv配置信息到临时目录,同步
                DownLoadSources("ABVersion.csv", TemCSVPath + "/ABVersion.csv");
                string e = File.ReadAllText(ABVersionPath);
                //解析下载的资源配置信息

                //将资源配置信息保存
                Dictionary<string, ABVersion> abVersionsDic = new Dictionary<string, ABVersion>();

                string[] str = e.Split('\n');
                //取出本地的配置信息,对比获取哪些版本不同,不同的要更新
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        ab = new ABVersion
                        {
                            AbName = content[0],
                            Version = int.Parse(content[1]),
                            MD5 = content[2].Trim()
                        };
                        abVersionsDic.Add(content[0], ab);
                    }
                }

                string temE = File.ReadAllText(TemCSVPath + "/ABVersion.csv");
                //更具版本更新资源

                str = temE.Split('\n');
                //需要更新的ab包
                List<string> allAbName = new List<string>();
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        string abName = content[0];
                        int version = int.Parse(content[1]);
                        if (abVersionsDic.ContainsKey(abName))
                        {
                            ab = abVersionsDic[abName];
                            //版本不同的才要更新
                            if (ab.Version != version)
                            {
                                allAbName.Add(ab.AbName);
                            }
                        }
                        else
                        {
                            allAbName.Add(abName);
                        }
                        //更新csv文件内容
                        ab = new ABVersion
                        {
                            AbName = abName,
                            Version = version,
                            MD5 = content[2].Trim()
                        };
                        abVersions.Add(ab);
                    }
                }
                //需要更新的文件数量
                _loadAbNum = allAbName.Count;

                if (_loadAbNum <= 0)
                {
                    callback?.Invoke();
                }

                //下载更新的文件
                for (int i = 0; i < allAbName.Count; i++)
                {
                    TimeManager.Instance.StartCoroutine(DownLoadABIE(allAbName[i]));
                }

                //有更新,需要将最新的资源配置信息保存
                if (_loadAbNum > 0)
                    ResponseExportCSV(abVersions, ABVersionPath);
            }
            else
            {
                //第一次将服务器上的的配置信息直接保存
                DownLoadSources("ABVersion.csv", ABVersionPath);

                string e = File.ReadAllText(ABVersionPath);

                string[] str = e.Split('\n');
                List<string> allAbName = new List<string>();
                for (int i = 1; i < str.Length; i++)
                {
                    string line = str[i];
                    if (line != "")
                    {
                        string[] content = line.Split(',');
                        allAbName.Add(content[0]);
                        ab = new ABVersion
                        {
                            AbName = content[0],
                            Version = int.Parse(content[1]),
                            MD5 = content[2].Trim()
                        };
                        abVersions.Add(ab);
                    }
                }
                //下载的ab包资源数量
                _loadAbNum = allAbName.Count;
                if (_loadAbNum <= 0)
                {
                    callback?.Invoke();
                }
                //下载
                for (int i = 0; i < allAbName.Count; i++)
                {
                    TimeManager.Instance.StartCoroutine(DownLoadABIE(allAbName[i]));
                }
                ResponseExportCSV(abVersions, ABVersionPath);
            }

        }
        /// <summary>
        /// 当所有资源下载完成或者更新完成,开始游戏
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        IEnumerator DownLoadABIE(string abName)
        {
            yield return null;
            DownLoadAB(abName);
            _loadAbNum--;
            if (_loadAbNum <= 0)
            {
                callback?.Invoke();
            }
        }

        /// <summary>
        /// 将ssv写入到指定目录
        /// </summary>
        /// <param name="abVersions"></param>
        /// <param name="fileName"></param>
        void ResponseExportCSV(List<ABVersion> abVersions, string fileName)
        {
            if (fileName.Length > 0)
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string dataHeard = string.Empty;
                //这个地方是写入CSV的标题栏 注意最后个没有分隔符
                dataHeard = "AbName,Version,MD5";
                sw.WriteLine(dataHeard);
                //写入数据
                for (int i = 0; i < abVersions.Count; i++)
                {
                    string dataStr = string.Format("{0},{1},{2}", abVersions[i].AbName, abVersions[i].Version, abVersions[i].MD5);
                    sw.WriteLine(dataStr);
                }
                sw.Close();
                fs.Close();
            }
        }


        private void CopyDirToLocal(string streamingAssetsPath, string targetPath, string filePath)
        {
            TimeManager.Instance.StartCoroutine(ILoadAndDownAbPackageFile(streamingAssetsPath, targetPath, filePath));
        }

        IEnumerator ILoadAndDownAbPackageFile(string abPath, string downLoadPath, string filePath)
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();
            string str = request.downloadHandler.text;
            string[] abSources = str.Split(',');
            int allNum = abSources.Length;
            int indexNum = 0;
            string downLoadTruePath;
            for (int i = 0; i < allNum; i++)
            {
                string loadPath = string.Format("{0}/{1}", abPath, abSources[i]);
                downLoadTruePath = string.Format("{0}/{1}", downLoadPath, abSources[i]);
                request = UnityWebRequest.Get(loadPath);
                yield return request.SendWebRequest();
                downloadAndSave(request, downLoadTruePath);
                if (++indexNum == allNum)
                {
                    _isLoadAB = true;
                }
            }

            yield return new WaitUntil(() => _isLoadAB);
        }


        public void DownLoadAB(string AbSources)
        {
            //本地有没有,有的话更新,没有就读取streamingAssetsPath
            //有就根据版本更新,没有就去streamingAssets下载完整ab资源
            //更新完成或者下载完成,才开始进入游戏
            string loadPath = Application.persistentDataPath + "/ABPackage";
            if (!Directory.Exists(loadPath))
                Directory.CreateDirectory(loadPath);

            string userName = "";
            string password = "";

            downloadWithFTP("ftp://192.168.1.12/" + AbSources, loadPath + "/" + AbSources, userName: userName, password: password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources">文件</param>
        /// <param name="loadPath">路径</param>
        public void DownLoadSources(string sources, string loadPath)
        {
            string userName = "";
            string password = "";

            downloadWithFTP("ftp://192.168.1.12/" + sources, loadPath, userName: userName, password: password);
        }

        private byte[] downloadWithFTP(string ftpUrl, string savePath = "", string userName = "", string password = "")
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUrl));
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                request.Credentials = new NetworkCredential(userName, password);
            }

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            if (!string.IsNullOrEmpty(savePath))
            {
                downloadAndSave(request.GetResponse(), savePath);
                return null;
            }
            else
            {
                return downloadAsbyteArray(request.GetResponse());
            }
        }

        byte[] downloadAsbyteArray(WebResponse request)
        {
            using (Stream input = request.GetResponseStream())
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while (input.CanRead && (read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }

        void downloadAndSave(WebResponse request, string savePath)
        {
            Stream reader = request.GetResponseStream();
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            FileStream fileStream = new FileStream(savePath, FileMode.Create);

            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            while (true)
            {
                bytesRead = reader.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
        }

        void downloadAndSave(UnityWebRequest request, string savePath)
        {
            savePath = savePath.Trim();
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }
            FileStream fileStream = new FileStream(savePath, FileMode.Create);
            fileStream.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);
            fileStream.Close();
        }
    }
}