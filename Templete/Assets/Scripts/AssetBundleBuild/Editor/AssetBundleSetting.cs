using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace Asset
{
    public enum BuildType
    {
        All,
        Update
    }
    [Serializable]
    public class AssetBundleSetting : ScriptableObject
    {
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        [Header("忽略文件后缀")]
        public List<string> Ignore = new List<string>();
        [Header("含字段则为Mutil文件夹")]
        public List<string> Mutil_Contains = new List<string>();
        [Header("以字段结尾则为Mutil文件夹")]
        public List<string> Mutil_EndWith = new List<string>();
        [Header("单个文件打包的文件夹")]
        public List<string> Single = new List<string>();
        [Header("含有多个文件打包的文件夹")]
        public List<string> Mutil = new List<string>();
        [Header("忽略Delete")]
        public List<string> DontDelete = new List<string>();
        public bool UseEditor = true;
        [Header("输出路径")]
        public string OutPath;
        //[Header("输出忽略路径")]
        //public List<string> OutPathIgnore = new List<string>();
        //public BuildType buildType = BuildType.All;
        private static AssetBundleSetting _instance;
        private static readonly object obj=new object();
        public static AssetBundleSetting Instance
        {
            get
            {

                if (_instance == null)
                {
                    lock (obj)
                    {
                        _instance = Resources.Load<AssetBundleSetting>("AssetBundleSetting");
                    }

                }
                return _instance;
            }
        }
        public string GetKeyByPath(string path)
        {
            string key = path.Replace("\\\\", ".").Replace("//", ".").Replace("\\", ".").Replace("/", ".");
            FileInfo fileInfo = new FileInfo(path);
            if (File.Exists(path) && fileInfo.Extension!="") key = key.Replace(fileInfo.Extension, "");
            return key;
        }
    }
}