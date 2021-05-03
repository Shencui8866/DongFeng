using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// AB包管理器
/// </summary>
public class ABManager : MonoSingletonBase<ABManager>
{
    /// <summary>
    /// 主包
    /// </summary>
    private AssetBundle mainAB;

    /// <summary>
    /// 依赖包配置文件
    /// </summary>
    private AssetBundleManifest manifest;

    /// <summary>
    /// 存储AB包字典，  key：包名  value：包
    /// </summary>
    private Dictionary<string, AssetBundle> abDict = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// AB包路径
    /// </summary>
    private string PathUrl => Application.streamingAssetsPath + "/";

    /// <summary>
    /// 主包名称
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
return "IOS";
#elif UNITY_ANDROID
return "Android";
#else
            return "StandaloneWindows";
#endif
        }
    }

    #region 公开方法

    #region 同步加载资源

    /// <summary>
    /// 同步加载资源,不指定类型
    /// </summary>
    /// <param name="abName">目标包名</param>
    /// <param name="resName">资源名</param>
    public Object LoadResources(string abName, string resName)
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        Object res = abDict[abName].LoadAsset(resName);
        //如果是GameObject 直接返回实例化
        if (res is GameObject)
        {
            return Instantiate(res);
        }

        return res;
    }

    /// <summary>
    /// 同步加载资源,指定类型
    /// </summary>
    /// <param name="abName">目标包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="type">类型</param>
    public Object LoadResources(string abName, string resName, Type type)
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        Object res = abDict[abName].LoadAsset(resName, type);
        //如果是GameObject 直接返回实例化
        if (res is GameObject)
        {
            return Instantiate(res);
        }

        return res;
    }

    /// <summary>
    /// 同步加载资源,指定类型(泛型)
    /// </summary>
    /// <param name="abName">目标包名</param>
    /// <param name="resName">资源名</param>
    public T LoadResources<T>(string abName, string resName) where T : Object
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        T res = abDict[abName].LoadAsset<T>(resName);
        //如果是GameObject 直接返回实例化
        if (res is GameObject)
        {
            return Instantiate(res);
        }

        return res;
    }

    #endregion

    #region 异步加载资源（AB包同步加载，资源异步加载）

    #region 根据名称加载

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="abName">包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="callback">回调</param>
    public void LoadResourcesAsync(string abName, string resName, UnityAction<Object> callback)
    {
        StartCoroutine(ReallyLoadResourcesAsync(abName, resName, callback));
    }

    private IEnumerator ReallyLoadResourcesAsync(string abName, string resName, UnityAction<Object> callback)
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        AssetBundleRequest res = abDict[abName].LoadAssetAsync(resName);
        yield return res;

        //异步加载结束后，通过委托传递出去
        callback(res.asset is GameObject ? Instantiate(res.asset) : res.asset);
    }

    #endregion

    #region 根据Type加载

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="abName">包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="type">类型</param>
    /// <param name="callback">回调</param>
    public void LoadResourcesAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
    {
        StartCoroutine(ReallyLoadResourcesAsync(abName, resName, type, callback));
    }

    private IEnumerator ReallyLoadResourcesAsync(string abName, string resName, System.Type type,
        UnityAction<Object> callback)
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        AssetBundleRequest res = abDict[abName].LoadAssetAsync(resName, type);
        yield return res;

        //异步加载结束后，通过委托传递出去
        callback(res.asset is GameObject ? Instantiate(res.asset) : res.asset);
    }

    #endregion

    #region 根据泛型加载

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="abName">包名</param>
    /// <param name="resName">资源名</param>
    /// <param name="callback">回调</param>
    public void LoadResourcesAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(ReallyLoadResourcesAsync<T>(abName, resName, callback));
    }

    private IEnumerator ReallyLoadResourcesAsync<T>(string abName, string resName, UnityAction<T> callback)
        where T : Object
    {
        //加载需要用到的包
        LoadAB(abName);

        //加载资源
        AssetBundleRequest res = abDict[abName].LoadAssetAsync<T>(resName);
        yield return res;

        //异步加载结束后，通过委托传递出去
        callback(res.asset is GameObject ? Instantiate(res.asset) as T : res.asset as T);
    }

    #endregion

    #endregion

    /// <summary>
    /// 卸载指定AB包
    /// </summary>
    /// <param name="abName">包名</param>
    /// <param name="isHierarchy">是否将正在使用的AB包资源卸载</param>
    public void UnLoadAssetBundle(string abName, bool isHierarchy = false)
    {
        if (abDict.ContainsKey(abName))
        {
            abDict[abName].Unload(isHierarchy);
            abDict.Remove(abName);
        }
    }

    /// <summary>
    /// 卸载所有AB包
    /// </summary>
    /// <param name="isHierarchy">是否将正在使用的AB包资源卸载</param>
    public void ClearAllAB(bool isHierarchy = false)
    {
        AssetBundle.UnloadAllAssetBundles(isHierarchy);
        abDict.Clear();
        mainAB = null;
        manifest = null;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 加载主包、目标包、依赖包
    /// </summary>
    /// <param name="abName">目标名称</param>
    private void LoadAB(string abName)
    {
        if (mainAB == null)
        {
            //加载主包
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            //加载主包依赖配置信息
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        //获取目标包的依赖包依赖包名
        string[] dependNames = manifest.GetAllDependencies(abName);

        //加载依赖包
        foreach (var dependName in dependNames)
        {
            //判断包是否已经被加载
            if (!abDict.ContainsKey(dependName))
            {
                //加载
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + dependName);
                //存储到字典中
                abDict.Add(dependName, ab);
            }
        }

        //加载目标包
        if (!abDict.ContainsKey(abName))
        {
            AssetBundle targetAB = AssetBundle.LoadFromFile(PathUrl + abName);
            //存储到字典中
            abDict.Add(abName, targetAB);
        }
    }

    #endregion
}