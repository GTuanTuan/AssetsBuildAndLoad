using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Asset
{
    public enum AssetType
    {
        Single = 1,
        Multi = 2     
    }

    public class AssetInfo
    {
        public string root;
        public string name;
        public AssetType type;
        public long LastWriteTime;

        private string fullname;
        public string FullName { get { fullname = root + name;  return fullname; }set { fullname = value; } }  // 全路径
        public List<AssetInfo> subFiles = new List<AssetInfo>(); // 孩子文件

        private string targetPath = null;
        public string TargetPath
        {
            get
            {
                if (targetPath != null)
                {
                    return targetPath;
                }
                else
                {
                    var originName = name;
                    if (name.Contains("."))
                    {
                        originName = name.Substring(0, name.LastIndexOf("."));
                    }
                    var _root = root.ToLower();
                    if (type == AssetType.Multi)
                    {
                        targetPath = _root.Substring(0, root.Length - 1) + ".unity3d";
                    }
                    else
                    {
                        targetPath = _root + originName.ToLower() + ".unity3d";
                    }
                    return targetPath;
                }
            }
        }

        public AssetBundleBuild GetAssetBundle()
        {
            if (type == AssetType.Single)
            {
                return new AssetBundleBuild
                {
                    assetBundleName = TargetPath, 
                    assetNames = new string[] { "Assets/" + FullName }
                };
            }
            else
            {

                List<string> _assetNames = new List<string>();
                foreach (AssetInfo file in subFiles)
                {
                    _assetNames.Add("Assets/" + file.FullName);
                }
                return new AssetBundleBuild
                {
                    assetBundleName = TargetPath,
                    assetNames = _assetNames.ToArray()
                };

            }
        }
        public bool IsChange(FileInfo fileInfo)
        {
            //return true;
            AssetBuildEnv.real_assetDict[FullName]= this;
            if (File.Exists(fileInfo.FullName + ".meta"))
            {
                FileInfo meta = new FileInfo(fileInfo.FullName + ".meta");
                if(meta.LastWriteTime.ToFileTime()> LastWriteTime)
                {
                    LastWriteTime = meta.LastWriteTime.ToFileTime();
                }
            }
            bool ischange = (!AssetBuildEnv.LastAssetMap.ContainsKey(FullName) ||               //上次打包没有存在，是这次新增的
                (AssetBuildEnv.LastAssetMap[FullName].LastWriteTime != LastWriteTime)); //文件有进行修改的
            if (ischange)
            {
                Debug.Log($"更新:{FullName}时间:{LastWriteTime}");
                AssetBuildEnv.LastAssetMap[FullName] = this;
            }
            return ischange;
        }
    }
    
}