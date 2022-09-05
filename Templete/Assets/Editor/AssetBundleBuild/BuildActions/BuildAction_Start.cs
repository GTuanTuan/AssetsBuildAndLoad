
using UnityEngine;

namespace Asset
{
    public class BuildAction_Start : BuildAction
    {
        public BuildAction_Start()
        {
            Debug.Log("<color=yellow>BuildMechine</color> -> <color=orange>" + this.GetType().Name + "</color>");
            AssetBuildEnv.Clear();
        }
        public override BuildState OnUpdate()
        {
            return BuildState.Success;
        }
    }
}