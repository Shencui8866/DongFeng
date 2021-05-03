using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTest : MonoBehaviour
{
    void Start()
    {
        // AssetBundle ab_model = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "model");
        //
        // //加载主包 
        // AssetBundle ab_Main = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "StandaloneWindows");
        // //获取依赖信息文件
        // AssetBundleManifest manifest = ab_Main.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // //获取依赖信息
        // string[] dependenciesInfo = manifest.GetAllDependencies("model");
        //
        // foreach (var info in dependenciesInfo)
        // {
        //     AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + info);
        // }
        //
        //
        // GameObject cube = ab_model.LoadAsset<GameObject>("Cube");
        // Instantiate(cube);

        //同步加载的3中方式
        GameObject cube = ABManager.Instance.LoadResources("model", "Cube") as GameObject;
        cube.transform.position = Vector3.up;

        GameObject cube2 = ABManager.Instance.LoadResources("model", "Cube", typeof(GameObject)) as GameObject;
        cube2.transform.position = Vector3.down;

        GameObject cube3 = ABManager.Instance.LoadResources<GameObject>("model", "Cube");
        cube3.transform.position = Vector3.left;


        //异步加载的3中方式
        ABManager.Instance.LoadResourcesAsync("model", "Cube",
            (p) => { ((GameObject) p).transform.position = new Vector3(0, 3, 0); });

        ABManager.Instance.LoadResourcesAsync("model", "Cube", typeof(GameObject),
            (p) => { ((GameObject) p).transform.position = new Vector3(0, -3, 0); });

        ABManager.Instance.LoadResourcesAsync<GameObject>("model", "Cube",
            (p) => { p.transform.position = new Vector3(-3, 0, 0); });
    }
}