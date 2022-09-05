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
        public List<string> Ignore = new List<string>();
        public List<string> Mutil_Contains = new List<string>();
        public List<string> Mutil_EndWith = new List<string>();
        public List<string> Single = new List<string>();
        public List<string> Mutil = new List<string>();
        public string OutPath;
        public BuildType buildType = BuildType.All;
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
    }
}