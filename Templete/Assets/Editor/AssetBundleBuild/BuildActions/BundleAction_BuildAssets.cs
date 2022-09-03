using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Asset
{

    public class BundleAction_BuildAssets : BuildAction
    {

        public override BuildState OnUpdate()
        {
            BuildAssetBundles();
            return BuildState.Success;
        }

        void BuildAssetBundles()
        {
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            foreach (KeyValuePair<string, AssetInfo> dict in AssetBuildEnv.assetDict)
            {
                var info = dict.Value;
                AssetBundleBuild build = dict.Value.GetAssetBundle();
                if (build.assetNames.Length > 0)
                {
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
            StringBuilder asset_map = new StringBuilder("");
            foreach (var keyValue in AssetBuildEnv.assetDict)
            {
                if(keyValue.Value.subFiles!=null && keyValue.Value.subFiles.Count > 0)
                {
                    foreach (var file in keyValue.Value.subFiles)
                    {
                        Debug.Log("更新:" + file.FullName);
                        asset_map.AppendLine(file.FullName + "|" + file.LastWriteTime);
                    }
                }
                else
                {
                    Debug.Log("更新:" + keyValue.Key);
                    asset_map.AppendLine(keyValue.Key + "|" + keyValue.Value.LastWriteTime);
                }
            }
            File.WriteAllText($"{AssetBuildEnv.setting.OutPath}/asset_map.txt", asset_map.ToString());
        }
    }
}