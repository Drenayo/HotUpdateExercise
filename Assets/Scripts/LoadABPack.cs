using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadABPack : MonoBehaviour
{
    public string abName;
    public string resName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("重新加载资源包");
            LoadAB();
        }
    }
    public void LoadAB()
    {
        // 加载AB包
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abName);

        // 如果一个资源身上用了其他AB包的资源，这时如果只加载了自身的AB包，则会出现资源丢失，必须要把其他依赖的AB包也一起加载了
        // 加载依赖包可以利用主包来获取依赖信息

        // 加载主包
        AssetBundle abMain = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "PC");
        // 加载主包中的固定文件（存储依赖信息）
        AssetBundleManifest abManifest = abMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // 从固定文件中获取某AB包的所有依赖项
        string[] str = abManifest.GetAllDependencies(abName);

        // 加载依赖项
        for (int i = 0; i < str.Length; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + str[i]);
        }

        // 把依赖包加载出来，会自动去加载依赖项
        Instantiate(ab.LoadAsset(resName));
        AssetBundle.UnloadAllAssetBundles(false);

    }
}