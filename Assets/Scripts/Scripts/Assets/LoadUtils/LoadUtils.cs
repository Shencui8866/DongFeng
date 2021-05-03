using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing;

namespace Josing
{
    public class LoadUtils : SingletonBase<LoadUtils>
    {
        Dictionary<string, GameObject> Pool = new Dictionary<string, GameObject>();

        /// <summary>
        /// 加载AB资源
        /// </summary>
        /// <param name="pathName">资源路径（相对路径）</param>
        /// <param name="assetName">资源名字</param>
        /// <param name="OnAsset">委托，需要对内存中的资源进行实例</param>
        /// <returns></returns>
        public IEnumerator LoadAssets(string pathName, string assetName, Action<GameObject> OnAsset)
        {
            GameObject go;
            AssetBundleRequest request = null;
            if (!Pool.ContainsKey(pathName))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + pathName);
                request = ab.LoadAssetAsync<GameObject>(assetName);
                yield return request;
                Pool.Add(pathName, request.asset as GameObject);
            }
            go = Pool[pathName];
            yield return null;
            if (OnAsset != null)
                OnAsset(go);
        }
    }
}
