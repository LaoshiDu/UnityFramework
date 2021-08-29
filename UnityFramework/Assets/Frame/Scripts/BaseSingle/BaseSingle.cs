#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 不用挂载的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMgr<T> where T : class, new()
    {
        public EventCall callback;
        private static volatile T instance;
        // 用于lock块的对象
        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
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
        public EventCall callback;
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
#if UNITY_EDITOR
                    if (EditorApplication.isPlaying)
                    {
                        DontDestroyOnLoad(singleton);
                    }
#else
                    DontDestroyOnLoad(singleton);
#endif
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