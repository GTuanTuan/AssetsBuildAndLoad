using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asset
{
    public abstract class BuildAction 
    {
        public BuildTimer Timer = new BuildTimer();
        public abstract BuildState OnUpdate();
        public virtual BuildProgress GetProgress()
        {
            return null;
        }
        public void OnActionEnter()
        {
            Timer.StartTime = DateTime.Now.Ticks;
            Debug.Log("<color=yellow>BuildMechine</color> -> <color=orange>" + this.GetType().Name + "</color>");
        }
        public void OnActionEnd()
        {
            Timer.EndTime = DateTime.Now.Ticks;
        }
    }
}