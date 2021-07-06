using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    [MenuItem("AssetBundleTools/BuildAllAssetBundles")]
    public static void BuildAllAB()
    {
        // 打包AB输出路径
        string strABOutPAthDir = string.Empty;

        // 获取“StreamingAssets”文件夹路径（不一定这个文件夹，可自定义）
        strABOutPAthDir = Application.streamingAssetsPath;

        // 判断文件夹是否存在，不存在则新建
        if (Directory.Exists(strABOutPAthDir) == false)
        {
            Directory.CreateDirectory(strABOutPAthDir);
        }

        // 打包生成AB包 (目标平台根据需要设置即可)
        BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        AssetDatabase.Refresh();
    }

    [MenuItem("AssetBundleTools/BuildAllAssetBundles2")]
    static void BuildMapABs()
    {
        string strABOutPAthDir = string.Empty;
        strABOutPAthDir = Application.streamingAssetsPath;
        // 判断文件夹是否存在，不存在则新建
        if (Directory.Exists(strABOutPAthDir) == false)
        {
            Directory.CreateDirectory(strABOutPAthDir);
        }

        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

        buildMap[0].assetBundleName = "video";

        string[] enemyAssets = new string[1];
        enemyAssets[0] = "Assets/Raw/Video/MyClip.mp4";

        buildMap[0].assetNames = enemyAssets;

        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets_mp4", buildMap, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        AssetDatabase.Refresh();
    }
}

