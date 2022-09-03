using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Asset
{
    public class BundleAction_Scan : BuildAction
    {
        public override BuildState OnUpdate()
        {
            Scan();
            return BuildState.Success;
        }
        void Scan()
        {
            if (AssetBuildEnv.assetDict == null)
            {
                AssetBuildEnv.assetDict = new Dictionary<string, AssetInfo>();
            }
            AssetBuildEnv.assetDict.Clear();
            foreach (string root in AssetBuildEnv.setting.Single)
            {
                string root1 = Application.dataPath+"/" + root;
                ScanSingle(root + "/", new DirectoryInfo(root1));
            }
            foreach (string root in AssetBuildEnv.setting.Mutil)
            {
                string root1 = Application.dataPath + "/" + root;
                ScanMutil(root + "/", new DirectoryInfo(root1));
            }
        }
        void ScanSingle(string root, DirectoryInfo dirInfo)
        {
            //当前文件夹的文件，不包括子文件夹内的文件
            FileInfo[] fileInfos = dirInfo.GetFiles();
            //当前文件夹下的所有文件夹，之后遍历使用
            DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
            if (fileInfos.Length > 0)
            {
                foreach (FileInfo file in fileInfos)
                {
                    //过滤忽略文件
                    if (IsIgnoreFile(file.Name)) continue;
                    AssetInfo info = new AssetInfo();
                    info.type = AssetType.Single;
                    info.LastWriteTime = file.LastWriteTime.ToFileTime();
                    info.root = root;
                    info.name = file.Name;
                    if(info.IsChange())
                        AssetBuildEnv.assetDict.Add(info.FullName, info);
                }
            }
            //遍历当前文件夹的下一层的子文件夹
            foreach (DirectoryInfo subdir in dirInfos)
            {
                ScanSingle(root + subdir.Name + "/", subdir);
            }
        }

        void ScanMutil(string root, DirectoryInfo dirInfo)
        {
            FileInfo[] fileInfos = dirInfo.GetFiles();
            DirectoryInfo[] dirInfos = dirInfo.GetDirectories();

            foreach (DirectoryInfo subdir in dirInfos)
            {
                ScanMutil(root + subdir.Name + "/", subdir);
            }
            if (fileInfos.Length > 0)
            {
                //单个文件打一个包
                if (!IsMutil(root))
                {
                    foreach (FileInfo file in fileInfos)
                    {
                        if (IsIgnoreFile(file.Name)) continue;
                        AssetInfo info = new AssetInfo();
                        info.type = AssetType.Single;
                        info.LastWriteTime = file.LastWriteTime.ToFileTime();
                        info.root = root;
                        info.name = file.Name;
                        if (info.IsChange())
                            AssetBuildEnv.assetDict.Add(info.FullName, info);
                    }
                }
                //多个文件打一个包
                else
                {
                    AssetInfo mInfo = new AssetInfo();
                    mInfo.type = AssetType.Multi;
                    mInfo.root = root;
                    mInfo.name = dirInfo.Name;
                    bool ischange = false;
                    foreach (FileInfo file in fileInfos)
                    {
                        if (IsIgnoreFile(file.Name)) continue;
                        AssetInfo info = new AssetInfo();
                        info.type = AssetType.Single;
                        info.LastWriteTime = file.LastWriteTime.ToFileTime();
                        info.root = root;
                        info.name = file.Name;
                        mInfo.subFiles.Add(info);
                        if (info.IsChange()) 
                            ischange = true;
                    }
                    if(ischange)
                        AssetBuildEnv.assetDict.Add(mInfo.FullName, mInfo);
                }
            }
        }
        bool IsMutil(string dir)
        {
            foreach (string search in AssetBuildEnv.setting.Mutil_Contains)
            {
                if (dir.Contains(search))
                {
                    return false;
                }
            }
            foreach (string endwith in AssetBuildEnv.setting.Mutil_EndWith)
            {
                if (dir.EndsWith(endwith))
                {
                    return false;
                }
            }
            return true;
        }
        bool IsIgnoreFile(string fileName)
        {
            foreach (string endwith in AssetBuildEnv.setting.Ignore)
            {
                if (fileName.EndsWith(endwith))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
