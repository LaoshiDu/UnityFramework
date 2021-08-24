using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class ObjectPoolManager : BaseMgr<ObjectPoolManager>
    {
        Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
        string suffixName = "(Clone)";
        GameObject objectPool;

        Dictionary<int, ObjectPoolConfig> objectPoolConfigDic;

        public void Init()
        {
            objectPool = GameObject.Find("ObjectPool");
            if (objectPool == null)
            {
                objectPool = new GameObject("ObjectPool");
                GameObject.DontDestroyOnLoad(objectPool);
            }
            objectPoolConfigDic = ObjectPoolConfig.GetDic();

            foreach (var item in objectPoolConfigDic)
            {
                List<GameObject> pool = new List<GameObject>();
                for (int i = 0; i < item.Value.PreloadAmount; i++)
                {
                    GameObject obj = null;

                    switch (GameConfig.Instance.loadType)
                    {
                        case AssetLoadType.Resources:
                            obj = GetObj(item.Value.resPath);
                            break;
                        case AssetLoadType.AssetBundle:
                            obj = GetObj(item.Value.ABName);
                            break;
                    }
                    pool.Add(obj);
                }
                for (int i = 0; i < pool.Count; i++)
                {
                    RecycleObject(pool[i]);
                }
            }
        }

        /// <summary>
        /// 获取游戏对象
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public GameObject GetObj(string Name)
        {
            string[] names = Name.Split('#');

            GameObject go;
            if (pool.ContainsKey(names[1] + suffixName))
            {
                if (pool[names[1] + suffixName].Count > 0)
                {
                    go = pool[names[1] + suffixName].Dequeue();
                    go.SetActive(true);
                }
                else
                {
                    GameObject obj = null;
                    switch (GameConfig.Instance.loadType)
                    {
                        case AssetLoadType.Resources:
                            obj = Resources.Load<GameObject>(names[0] + "/" + names[1]);
                            break;
                        case AssetLoadType.AssetBundle:
                            obj = AssetBundleManager.Instance.LoadRes<GameObject>(names[0], names[1]);
                            break;
                    }
                    go = GameObject.Instantiate(obj);
                }
            }
            else
            {
                GameObject obj = null;
                switch (GameConfig.Instance.loadType)
                {
                    case AssetLoadType.Resources:
                        obj = Resources.Load<GameObject>(names[0] + "/" + names[1]);
                        break;
                    case AssetLoadType.AssetBundle:
                        obj = AssetBundleManager.Instance.LoadRes<GameObject>(names[0], names[1]);
                        break;
                }
                pool.Add(names[1] + suffixName, new Queue<GameObject>());
                go = GameObject.Instantiate(obj);
            }
            return go;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="go">需要回收的对象</param>
        public void RecycleObject(GameObject go)
        {
            if (pool.ContainsKey(go.name))
            {
                pool[go.name].Enqueue(go);
            }
            else
            {
                pool.Add(go.name, new Queue<GameObject>());
                pool[go.name].Enqueue(go);
            }
            go.transform.SetParent(objectPool.transform);
            go.SetActive(false);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void ClearObjPool()
        {
            MyTools.DestroyAllChild(objectPool.transform);
            pool.Clear();
        }
    }
}