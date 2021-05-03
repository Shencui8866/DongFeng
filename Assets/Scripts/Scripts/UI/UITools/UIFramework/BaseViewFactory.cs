using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 2020 07 by josing
 * 
 * 加载路径按照 命名空间+类名（也就是预制体名称）来加载，请按照该方式书写命名空间
 */

namespace Josing.UI.Framework
{
    public class BaseViewFactory
    {
        public static Transform Active;
        public static Transform Idle;
        static Stack<BaseView> _viewStack = new Stack<BaseView>();
        static Dictionary<string, BaseView> _viewPools = new Dictionary<string, BaseView>();
        static List<ViewCache> _viewCache = new List<ViewCache>();

        /// <summary>
        /// 加载页面
        /// </summary>
        /// <param name="view"></param>
        public static void LoadView(Type view)
        {
            if(!Active || !Idle)
            {
                Debug.LogError("请设置路径和父物体！");
                return;
            }

            BaseView v = null;
            string path = GetPath(view);

            if (_viewStack.Count > 0) _viewStack.Peek().OnFreeze();

            if (!_viewPools.ContainsKey(path))
                _viewPools.Add(path, Resources.Load<GameObject>(path).GetComponent<BaseView>());

            if (GetCacheView(path, ref v) == null)
            {
                v = GameObject.Instantiate(_viewPools[path], Active).GetComponent<BaseView>();
                v.transform.localPosition = Vector3.zero;
                v.transform.localScale = Vector3.one;
                v.OnFirstLoad();
            }
            else v.transform.parent = Active;

            v.OnEnter();
            _viewStack.Push(v);
        }
        /// <summary>
        /// 退出页面
        /// </summary>
        /// <param name="view"></param>
        public static void ExitView(Type view)
        {
            if (!Active || !Idle)
            {
                Debug.LogError("请设置路径和父物体！");
                return;
            }

            if (_viewStack.Count > 1)
            {
                BaseView v = _viewStack.Pop();
                v.OnExit();
                v.transform.parent = Idle;
                _viewCache.Add(new ViewCache() { path = GetPath(view), view = v });

                _viewStack.Peek().OnResume();
            }
        }
        /// <summary>
        /// 获取当前显示页面
        /// </summary>
        /// <returns></returns>
        public static BaseView GetCurView() { return _viewStack.Peek(); }

        static string GetPath(Type view)
        {
            return view.FullName.Replace(".", "/");
        }

        static BaseView GetCacheView(string path, ref BaseView v)
        {
            ViewCache viewCache = _viewCache.Find((x) =>
            {
                if (x.path.Equals(path))
                    return true;
                return false;
            });
            if (viewCache != null)
            {
                _viewCache.Remove(viewCache);
                v = viewCache.view;
                return viewCache.view;
            }
            return null;
        }

        class ViewCache
        {
            public BaseView view;
            public string path;
        }
    }
}

