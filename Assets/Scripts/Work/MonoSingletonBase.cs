using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonoSingletonBase<T> : MonoBehaviour
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
                GameObject g = new GameObject(typeof(T).Name);
                _instance = g.AddComponent<T>();
            }

            return _instance;
        }
    }
}

public class SingletonBase<T> where T : class, new()
{
    private static T instance;

    public static T Instance => instance ?? (instance = new T());
}