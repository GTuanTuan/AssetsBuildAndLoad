using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Asset
{

    public class BuildAction_BuildAssets : BuildAction
    {

        public override BuildState OnUpdate()
        {
            BuildAssetBundles();
            SaveLastAssetMap();
            return BuildState.Success;
        }

        void BuildAssetBundles()
        {
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            //Dictionary<string, AssetInfo> assetDict = new Dictionary<string, AssetInfo>();
            //if (AssetBuildEnv.setting.buildType == BuildType.All)
            //{
            //    assetDict = AssetBuildEnv.all_assetDict;
            //}
            //else
            //{
            //    //assetDict = AssetBuildEnv.all_assetDict;
            //    assetDict = AssetBuildEnv.update_assetDict;
            //}
            Dictionary<string, AssetInfo> assetDict = AssetBuildEnv.all_assetDict;
            foreach (KeyValuePair<string, AssetInfo> dict in assetDict)
            {
                var info = dict.Value;
                AssetBundleBuild build = dict.Value.GetAssetBundle();
                if (build.assetNames.Length > 0)
                {
                    //foreach (string str in AssetBuildEnv.setting.OutPathIgnore)
                    //{
                    //    build.assetBundleName = build.assetBundleName.Replace(str, "");
                    //}
                    //build.assetBundleName = build.assetBundleName.Replace("_/","/");
                    list.Add(build);
                }
            }
            if (list.Count <= 0)
            {
                Debug.Log("无需打包");
                return;
            }
            string out_path = AssetBuildEnv.setting.OutPath; // 输出路径
            if (!Directory.Exists(out_path))
            {
                Directory.CreateDirectory(out_path);
            }
            // 开始打包
            BuildPipeline.BuildAssetBundles(out_path, list.ToArray(), BuildAssetBundleOptions.None, AssetBuildEnv.setting.buildTarget); 
        }
        void SaveLastAssetMap()
        {
            StringBuilder asset_map = new StringBuilder("");
            foreach (var keyValue in AssetBuildEnv.LastAssetMap)
            {
                if (keyValue.Value.subFiles != null && keyValue.Value.subFiles.Count > 0)
                {
                    foreach (var file in keyValue.Value.subFiles)
                    {
                        asset_map.AppendLine(file.FullName + "|" + file.LastWriteTime);
                    }
                }
                else
                {
                    asset_map.AppendLine(keyValue.Key + "|" + keyValue.Value.LastWriteTime);
                }
            }
            File.WriteAllText($"{AssetBuildEnv.setting.OutPath}/asset_map.txt", asset_map.ToString());
            AssetBuildEnv.LastAssetMap.Clear();
        }
    }
}