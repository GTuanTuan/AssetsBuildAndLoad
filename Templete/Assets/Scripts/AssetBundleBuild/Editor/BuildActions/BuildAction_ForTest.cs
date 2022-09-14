
using System.Threading.Tasks;
using UnityEngine;

namespace Asset
{
    public class BuildAction_ForTest : BuildAction
    {
        public override BuildState OnUpdate()
        {
            for (float i = 0; i < 20000*Time.deltaTime; i+= Time.deltaTime)
            {
            }
            return BuildState.Success;
        }
    }
}