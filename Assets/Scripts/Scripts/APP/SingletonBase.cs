using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing
{
    public class SingletonBase<T> : MonoBehaviour
    where T : MonoBehaviour
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject g = new GameObject("Instance -> (" + typeof(T).Name + ")");
                    _instance = g.AddComponent<T>();
                }
                //if(_instance != null) DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }
    }
}

