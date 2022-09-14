using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace Asset
{
    public class BuildMechine 
    {
        public static BuildMechine Instance;
        public bool AnyError;
        public List<BuildAction> Actions = new List<BuildAction>();
        public List<BuildTimer> ActionTimers = new List<BuildTimer>();
        //总时间
        public BuildTimer MechineTimer = new BuildTimer();
        //当前Action下标
        public int CurrentActionIndex;
        public BuildMechine()
        {
        }
        //获取当前Action
        public BuildAction CurrentBuildAction
        {
            get
            {
                return Actions.Count > CurrentActionIndex ? Actions[CurrentActionIndex] : null;
            }
        }
        public bool IsFinished
        {
            get { return CurrentBuildAction == null; }
        }

        public void UpdateMethod()
        {
            if (EditorApplication.isCompiling) return;
            if (CurrentBuildAction != null)
            {
                BuildState buildState;
                try
                {
                    buildState = CurrentBuildAction.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    buildState = BuildState.Failure;
                }
                switch (buildState)
                {
                    case BuildState.None:
                    case BuildState.Running:
                        break;
                    case BuildState.Success:
                        CurrentBuildAction.OnActionEnd();
                        CurrentActionIndex++;
                        if (CurrentBuildAction != null)
                        {
                            CurrentBuildAction.OnActionEnter();
                        }
                        else
                        {
                            BuildFinished(false);
                        }
                        break;
                    case BuildState.Failure:
                        Debug.LogError("<color=yellow>BuildMechine</color> : Build Fail!!!");
                        CurrentBuildAction.OnActionEnd();
                        BuildFinished(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        private void BuildFinished(bool anyError)
        {
            AnyError = anyError;
            this.MechineTimer.EndTime = DateTime.Now.Ticks;
            Debug.Log("Context : "  + this.MechineTimer.Duration);
        }
        public static BuildMechine NewPipeline()
        {
            Instance = new BuildMechine();
            Instance.Actions = new List<BuildAction>();
            Instance.Actions.Add(new BuildAction_Start());
            return Instance;
        }
        public BuildMechine AddActions(params BuildAction[] actions)
        {
            this.Actions.AddRange(actions);
            return this;
        }
        public void Run()
        {
            this.Actions.Add(new BuildAction_End());
            Instance.MechineTimer = new BuildTimer()
            {
                StartTime = DateTime.Now.Ticks
            };
            Instance.CurrentActionIndex = 0;
            var window = EditorWindow.GetWindow<BuildMechineWindows>();
            window.Focus();
        }
        public BuildProgress GetProgress()
        {
            if (CurrentBuildAction != null) return CurrentBuildAction.GetProgress();

            return null;
        }
        public static void ShowProgress()
        {
            if (Instance != null)
            {

                if (Instance.IsFinished)
                {

                    EditorWindow.GetWindow<BuildMechineWindows>().Close();
                    return;
                }
                var progress = Instance.GetProgress();
                if (progress != null)
                {

                    EditorUtility.DisplayProgressBar(progress.Title, progress.Content, progress.Porgress);
                }
            }
        }
    }
}