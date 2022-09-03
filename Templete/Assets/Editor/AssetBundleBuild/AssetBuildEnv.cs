using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Asset
{
    public class AssetBuildEnv
    {
        //public static string res_root = "Assets/res";
        public static string single_root = "Assets/res/single";
        public static string mutil_root = "Assets/res/mutil";
        public static Dictionary<string, AssetInfo> assetDict = new Dictionary<string, AssetInfo>();
        //上次打包的asset的信息,都分成单个
        private static Dictionary<string, AssetInfo> last_assetDict;
        public static AssetBundleSetting setting = AssetBundleSetting.Instance;
        public static Dictionary<string, AssetInfo> LastAssetMap
        {
            get
            {
                if (last_assetDict == null || last_assetDict.Count==0)
                {
                    last_assetDict = new Dictionary<string, AssetInfo>();
                    if (!File.Exists($"{AssetBuildEnv.setting.OutPath}/asset_map.txt")){ return null; }
                    string[] map_data = File.ReadAllLines($"{AssetBuildEnv.setting.OutPath}/asset_map.txt");
                    foreach (string data in map_data)
                    {
                        string key = data.Split('|')[0];
                        long time = long.Parse( data.Split('|')[1]);
                        AssetInfo assetInfo = new AssetInfo();
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