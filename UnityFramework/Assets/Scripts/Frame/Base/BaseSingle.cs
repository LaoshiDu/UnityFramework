using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 不用挂载的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMgr<T> where T : class, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }

    /// <summary>
    /// 挂载在GameObject上的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject singleton = GameObject.Find("SingletonObject");
                    if (singleton == null)
                    {
                        singleton = new GameObject("SingletonObject");
                    }
                    DontDestroyOnLoad(singleton);
                    _instance = singleton.GetComponent<T>();
                    if (_instance == null)
                    {
                        _instance = singleton.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }
}