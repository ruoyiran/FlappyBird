using UnityEngine;

namespace Utils
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        private static object _guard = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_guard)
                    {
                        if (_instance == null)
                        {
                            GameObject singletonObj = new GameObject();
                            _instance = singletonObj.AddComponent<T>();
                            singletonObj.name = "(Singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singletonObj);
                        }
                    }
                }
                return _instance;
            }
        }

        protected void Awake()
        {
            _instance = gameObject.GetComponent<T>();
            if (_instance == null)
                _instance = gameObject.AddComponent<T>();
            gameObject.name = "(Singleton) " + typeof(T).ToString();
            DontDestroyOnLoad(gameObject);
        }
    }

    public class Singleton<T> where T : new()
    {
        protected static T _instance;
        private static object _guard = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_guard)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}