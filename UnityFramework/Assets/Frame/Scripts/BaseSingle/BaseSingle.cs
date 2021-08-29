#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// ���ù��صĵ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMgr<T> where T : class, new()
    {
        public EventCall callback;
        private static volatile T instance;
        // ����lock��Ķ���
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
    /// ������GameObject�ϵĵ���
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