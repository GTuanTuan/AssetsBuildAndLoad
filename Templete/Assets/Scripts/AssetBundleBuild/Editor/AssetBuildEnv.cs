using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Asset
{
    public class AssetBuildEnv
    {
        //public static string res_root = "Assets/res";
        public static string single_root = "Assets/res/single";
        public static string mutil_root = "Assets/res/mutil";
        //这次要更新打包的asset的信息
        public static Dictionary<string, AssetInfo> update_assetDict = new Dictionary<string, AssetInfo>();
        //总的asset的信息
        public static Dictionary<string, AssetInfo> all_assetDict = new Dictionary<string, AssetInfo>();
        //总的asset的信息，都是单个
        public static Dictionary<string, AssetInfo> real_assetDict = new Dictionary<string, AssetInfo>();
        //没有用到的asset的信息，都是单个
        public static Dictionary<string, AssetInfo> useless_assetDict = new Dictionary<string, AssetInfo>();
        //上次打包的asset的信息,都是单个，在BuildAction_Scan后更新
        private static Dictionary<string, AssetInfo> last_assetDict = new Dictionary<string, AssetInfo>();
        public static AssetBundleSetting setting = AssetBundleSetting.Instance;
        public static void Clear()
        {
            real_assetDict.Clear();
            all_assetDict.Clear();
            useless_assetDict.Clear();
            update_assetDict.Clear();
            last_assetDict.Clear();
        }
        public static Dictionary<string, AssetInfo> LastAssetMap
        {
            get
            {
                if (last_assetDict == null || last_assetDict.Count==0)
                {
                    if (!File.Exists($"{AssetBuildEnv.setting.OutPath}/asset_map.txt")){ return last_assetDict; }
                    string[] map_data = File.ReadAllLines($"{setting.OutPath}/asset_map.txt");
                    foreach (string data in map_data)
                    {
                        string key = data.Split('|')[0];
                        long time = long.Parse( data.Split('|')[1]);
                        AssetInfo assetInfo = new AssetInfo();
                        if (File.Exists($"{setting.OutPath}/{key.Substring(0, key.LastIndexOf("."))}.unity3d"))
                        {
                            assetInfo.name = key.Substring(key.LastIndexOf("/")+1);
                            assetInfo.root = key.Remove( key.LastIndexOf("/")+1);
                            assetInfo.type = AssetType.Single;
                        }
                        else if(File.Exists($"{setting.OutPath}/{key.Substring(0, key.LastIndexOf("/"))}.unity3d"))
                        {
                            assetInfo.name = key.Substring(key.LastIndexOf("/") + 1);
                            assetInfo.root = key.Remove(key.LastIndexOf("/") + 1);
                            assetInfo.type = AssetType.Multi;
                        }
                        else
                        {
                            continue;
                        }
                        assetInfo.FullName = key;
                        assetInfo.LastWriteTime = time;
                        last_assetDict[key] = assetInfo;
                    }
                }
                return last_assetDict;
            }
        }
    }
}