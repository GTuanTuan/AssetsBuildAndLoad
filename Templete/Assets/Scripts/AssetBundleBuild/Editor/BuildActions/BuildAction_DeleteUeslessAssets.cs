using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Asset
{
    public class BuildAction_DeleteUeslessAssets : BuildAction
    {
        List<string> emptyDirs = new List<string>();
        public BuildAction_DeleteUeslessAssets()
        {

        }
        public override BuildState OnUpdate()
        {
            DeleteUeslessAssets();
            AddEmptyDirs(AssetBuildEnv.setting.OutPath);
            DeleteEmptyDirs();
            return BuildState.Success;
        }
        void DeleteEmptyDirs()
        {
            foreach (string dir in emptyDirs)
            {
                if (Directory.Exists(dir))
                {
                    Debug.Log($"删除文件夹{dir}");
                    Directory.Delete(dir);
                }
            }
        }
        bool AddEmptyDirs(string dirPath)
        {
            bool hasfile = false;
            if (Directory.GetFiles(dirPath).Length > 0) hasfile = true;
            foreach (string dir in Directory.GetDirectories(dirPath) )
            {
                if(AddEmptyDirs(dir)) hasfile = true;
            }
            if (!hasfile) emptyDirs.Add(dirPath);
            return hasfile;
        }
        void DeleteUeslessAssets()
        {
            foreach (KeyValuePair<string, AssetInfo> kv in AssetBuildEnv.LastAssetMap)
            {
                if (!AssetBuildEnv.real_assetDict.ContainsKey(kv.Key) && CheckCanDelete(kv.Value.FullName))
                {
                    Debug.Log($"删除文件{kv.Value.FullName}");
                    AssetBuildEnv.useless_assetDict[kv.Key] = kv.Value;
                    //if (kv.Value.type == AssetType.Single)
                    {
                        string assetPath = $"{AssetBuildEnv.setting.OutPath}/{kv.Value.TargetPath}";
                        DeleteAsset(assetPath);
                    }
                }
            }
            foreach (KeyValuePair<string, AssetInfo> kv in AssetBuildEnv.useless_assetDict)
            {
                if (AssetBuildEnv.LastAssetMap.ContainsKey(kv.Key))
                    AssetBuildEnv.LastAssetMap.Remove(kv.Key);
            }
        }
        void DeleteAsset(string assetPath)
        {
            string manifestPath = $"{assetPath}.manifest";
            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }
            if (File.Exists(manifestPath))
            {
                File.Delete(manifestPath);
            }
        }
        bool CheckCanDelete(string fullName)
        {
            bool can_delete = true;
            foreach (string str in AssetBuildEnv.setting.DontDelete)
            {
                if (fullName.Contains(str)) can_delete = false;
            }
            return can_delete;
        }
    }
}