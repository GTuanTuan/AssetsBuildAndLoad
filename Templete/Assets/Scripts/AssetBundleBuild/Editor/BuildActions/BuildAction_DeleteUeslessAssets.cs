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
            DeleteOtherAssets();
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
            emptyDirs.Clear();
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
        void DeleteOtherAssets()
        {
            string[] all_file_path = Directory.GetFiles(AssetBuildEnv.setting.OutPath, "*.*", SearchOption.AllDirectories);
            foreach (string file_path in all_file_path)
            {
                string _file_path = file_path.Replace(".manifest", "");
                string out_path_key = AssetBuildEnv.setting.GetKeyByPath(AssetBuildEnv.setting.OutPath+"/");
                string key = AssetBuildEnv.setting.GetKeyByPath(_file_path).Replace(out_path_key, "");
                bool useless = true;
                foreach (KeyValuePair<string, AssetInfo> assetDict in AssetBuildEnv.all_assetDict)
                {
                    string use_key = AssetBuildEnv.setting.GetKeyByPath(assetDict.Value.TargetPath.Replace(".unity3d",""));
                    if (key == use_key) useless = false;
                }
                if (useless && CheckCanDelete(_file_path)) 
                {
                    Debug.Log($"path:{_file_path},key:{key}");
                    DeleteAsset(_file_path); 
                }
            }
        }
        void DeleteUeslessAssets()
        {
            foreach (KeyValuePair<string, AssetInfo> kv in AssetBuildEnv.LastAssetMap)
            {
                if (!AssetBuildEnv.real_assetDict.ContainsKey(kv.Key) && CheckCanDelete(kv.Value.FullName))
                {
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
                Debug.Log($"删除文件{assetPath}");
                File.Delete(assetPath);
            }
            if (File.Exists(manifestPath))
            {
                Debug.Log($"删除文件{manifestPath}");
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
            string out_path_key = AssetBuildEnv.setting.GetKeyByPath(AssetBuildEnv.setting.OutPath + "/");
            string key = AssetBuildEnv.setting.GetKeyByPath(fullName).Replace(out_path_key, "");
            if (key == "AssetsBuildAndLoad" || key == "asset_map") can_delete = false;
            return can_delete;
        }
    }
}