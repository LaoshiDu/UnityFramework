using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WKC
{
    public class AssetBundleManager : BaseMgr<AssetBundleManager>
    {
        private Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();

        private AssetBundle mainAB = null;
        private AssetBundleManifest mainManifest = null;

        /// <summary>
        /// AB包路径
        /// </summary>
        private string ABFilePath
        {
            get
            {
                return Application.persistentDataPath + "/ABPackage/";
            }
        }

        /// <summary>
        /// 主包名字
        /// </summary>
        private string MainABName
        {
            get
            {
                return "ABPackage";
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private AssetBundle LoadAB(string name)
        {
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(ABFilePath + MainABName);
                mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            string[] str = mainManifest.GetAllDependencies(name);

            for (int i = 0; i < str.Length; i++)
            {
                if (!bundleDic.ContainsKey(str[i]))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(ABFilePath + str[i]);
                    bundleDic.Add(str[i], ab);
                }
            }
            if (!bundleDic.ContainsKey(name))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(ABFilePath + name);
                bundleDic.Add(name, ab);
            }
            return bundleDic[name];
        }

        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        private void LoadABAsync(string name, UnityAction<AssetBundle> callback)
        {
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(ABFilePath + MainABName);
                mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            string[] str = mainManifest.GetAllDependencies(name);
            TimeManager.Instance.StartCoroutine(LoadDependenciesAB(name, str, callback));
        }

        private IEnumerator LoadDependenciesAB(string name, string[] str, UnityAction<AssetBundle> callback)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!bundleDic.ContainsKey(str[i]))
                {
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(ABFilePath + str[i]);
                    yield return abcr;
                    bundleDic.Add(str[i], abcr.assetBundle);
                }
            }
            if (!bundleDic.ContainsKey(name))
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(ABFilePath + name);
                yield return abcr;
                bundleDic.Add(name, abcr.assetBundle);
                callback(abcr.assetBundle);
            }
        }

        /// <summary>
        /// 从AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadRes<T>(string abName, string resName) where T : Object
        {
            AssetBundle ab = LoadAB(abName);
            return ab.LoadAsset<T>(resName);
        }

        /// <summary>
        /// 从AB包中加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public GameObject LoadRes(string abName, string resName)
        {
            AssetBundle ab = LoadAB(abName);
            return ab.LoadAsset<GameObject>(resName);
        }

        /// <summary>
        /// 从AB包中根据type加载资源（Lua）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadRes(string abName, string resName, System.Type type)
        {
            AssetBundle ab = LoadAB(abName);
            return ab.LoadAsset(resName, type);
        }

        /// <summary>
        /// 从AB包中异步加载资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callback"></param>
        public void LoadResAsync(string abName, string resName, UnityAction<Object> callback)
        {
            TimeManager.Instance.StartCoroutine(LoadAsyncRes(abName, resName, callback));
        }

        private IEnumerator LoadAsyncRes(string abName, string resName, UnityAction<Object> callback)
        {
            AssetBundleRequest abr = null;
            LoadABAsync(abName, (ab) =>
            {
                abr = ab.LoadAssetAsync(resName);
            });
            yield return abr;
            callback(abr.asset);
        }

        /// <summary>
        /// 从AB包中异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="callback"></param>
        public void LoadResAsync<T>(string abName, string resName, UnityAction<Object> callback)
        {
            TimeManager.Instance.StartCoroutine(LoadAsyncRes<T>(abName, resName, callback));
        }

        private IEnumerator LoadAsyncRes<T>(string abName, string resName, UnityAction<Object> callback)
        {
            AssetBundleRequest abr = null;
            LoadABAsync(abName, (ab) =>
            {
                abr = ab.LoadAssetAsync<T>(resName);
            });
            yield return abr;
            callback(abr.asset);
        }

        /// <summary>
        /// 从AB包中根据type异步加载资源（Lua）
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
        {
            TimeManager.Instance.StartCoroutine(LoadAsyncRes(abName, resName, type, callback));
        }

        private IEnumerator LoadAsyncRes(string abName, string resName, System.Type type, UnityAction<Object> callback)
        {
            AssetBundleRequest abr = null;
            LoadABAsync(abName, (ab) =>
            {
                abr = ab.LoadAssetAsync(resName, type);
            });
            yield return abr;
            callback(abr.asset);
        }

        /// <summary>
        /// 加载一个AB包中的所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName"></param>
        /// <returns></returns>
        public T[] LoadAllRes<T>(string abName) where T : Object
        {
            AssetBundle ab = LoadAB(abName);
            return ab.LoadAllAssets<T>();
        }

        /// <summary>
        /// 卸载没有被使用的资源
        /// </summary>
        public void RemoveMenory()
        {
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 卸载单个AB包
        /// </summary>
        /// <param name="name"></param>
        public void ClearOneAB(string name)
        {
            if (bundleDic.ContainsKey(name))
            {
                bundleDic[name].Unload(false);
                bundleDic.Remove(name);
            }
        }

        /// <summary>
        /// 卸载所有AB包
        /// </summary>
        public void ClearAll()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            bundleDic.Clear();
            mainAB = null;
            mainManifest = null;
        }
    }
}