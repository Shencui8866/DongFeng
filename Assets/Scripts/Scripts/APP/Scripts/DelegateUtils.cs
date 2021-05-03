using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace Josing.Reflection
{
    public class DelegateUtils
    {
        /// <summary>
        /// 获取目标委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <typeparam name="U">自定义特性类型</typeparam>
        /// <param name="target">目标实例</param>
        /// <returns></returns>
        public static List<T> CreateDelegate<T, U>(object target) where T : Delegate where U : Attribute
        {
            if (target == null) throw new NullReferenceException();
            var getMethods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<T> cache = new List<T>();
            foreach (var methodInfo in getMethods)
            {
                foreach (var attribute in methodInfo.GetCustomAttributes(false))
                {
                    U temp = attribute as U;
                    if (temp != null)
                    {
                        cache.Add((T)methodInfo.CreateDelegate(typeof(T), target));
                        break;
                    }
                }
            }
            return cache;
        }

        /// <summary>
        /// 获取已加载类型委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <typeparam name="U">自定义特性类型</typeparam>
        /// <param name="targets">目标实例集合，为MonoBehaviour类型</param>
        /// <returns></returns>
        public static List<T> CreateDelegates<T, U>(object[] targets) where T : Delegate where U : Attribute
        {
            var loadTypes = Assembly.GetCallingAssembly().GetTypes();
            List<T> cache = new List<T>();
            foreach (var type in targets)
            {
                var getMethods = type.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var methodInfo in getMethods)
                {
                    foreach (var attribute in methodInfo.GetCustomAttributes(false))
                    {
                        U temp = attribute as U;
                        try
                        {
                            if (temp != null)
                            {
                                cache.Add((T)methodInfo.CreateDelegate(typeof(T), type));
                                break;
                            }
                        }
                        catch(Exception e) { Debug.Log(e.Message + "  " + typeof(T) + "  " + methodInfo + "  " + type); }
                    }
                }
            }
            return cache;
        }

        /// <summary>
        /// 获取目标委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <typeparam name="U">自定义特性类型</typeparam>
        /// <param name="target">目标实例</param>
        /// <returns></returns>
        public static List<T> CreateDelegate<T, U>(object target, List<T> exits) where T : Delegate where U : Attribute
        {
            if (target == null) throw new NullReferenceException();
            var getMethods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<T> cache = new List<T>();
            foreach (var methodInfo in getMethods)
            {
                foreach (var attribute in methodInfo.GetCustomAttributes(false))
                {
                    U temp = attribute as U;
                    if (temp != null)
                    {
                        T t = (T)methodInfo.CreateDelegate(typeof(T), target);
                        if (exits.Find((x) => { return t == x; }) == null) cache.Add(t);
                        break;
                    }
                }
            }
            return cache;
        }

        /// <summary>
        /// 获取已加载类型委托
        /// </summary>
        /// <typeparam name="T">委托类型</typeparam>
        /// <typeparam name="U">自定义特性类型</typeparam>
        /// <param name="targets">目标实例集合，为MonoBehaviour类型</param>
        /// <returns></returns>
        public static List<T> CreateDelegates<T, U>(object[] targets, List<T> exits) where T : Delegate where U : Attribute
        {
            var loadTypes = Assembly.GetCallingAssembly().GetTypes();
            List<T> cache = new List<T>();
            foreach (var type in targets)
            {
                var getMethods = type.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var methodInfo in getMethods)
                {
                    foreach (var attribute in methodInfo.GetCustomAttributes(false))
                    {
                        U temp = attribute as U;
                        try
                        {
                            if (temp != null)
                            {
                                T t = (T)methodInfo.CreateDelegate(typeof(T), type);
                                if (exits.Find((x) => { return t == x; }) == null) cache.Add(t);
                                break;
                            }
                        }
                        catch (Exception e) { Debug.Log(e.Message + "  " + typeof(T) + "  " + methodInfo + "  " + type); }
                    }
                }
            }
            return cache;
        }
    }

    public class FindObjectOfAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">自定义特性</typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static Object[] GetObjects<T>(Object[] objects) where T : Attribute
        {
            List<Object> types = new List<Object>();
            foreach (var o in objects)
            {
                foreach (var attr in o.GetType().GetCustomAttributes<T>())
                    if (attr != null)
                        types.Add(o);
            }
            return types.ToArray();
        }
    }
}

