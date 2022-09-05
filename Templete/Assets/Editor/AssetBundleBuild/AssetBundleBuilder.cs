using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Asset
{
    public class AssetBundleBuilder 
    {
        [MenuItem("资源/AssetBundle/Build")]
        public static void Build()
        {
            BuildAssets(EditorUserBuildSettings.selectedBuildTargetGroup, EditorUserBuildSettings.activeBuildTarget);
        }
        static void BuildAssets(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            BuildMechine.NewPipeline().AddActions(
                //new BuildAction_ForTest(),
                new BuildAction_Scan(),
                new BuildAction_DeleteUeslessAssets(),
                new BuildAction_BuildAssets()
            ).Run();
        }
        [MenuItem("快捷键/Special Command %g")]
        public static void Test()
        {
            Build();
        }
    }
}
