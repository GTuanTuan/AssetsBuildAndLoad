using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Asset
{
    public class BuildMechineWindows : EditorWindow
    {
        private class BuildInfo
        {
            public int Index;
            public string ActionName;
            public string State;
            public TimeSpan Duration;
        }

        private GUIStyle _fontStyle;
        private GUIStyle _titleStyle;
        private List<BuildInfo> _buildInfos = new List<BuildInfo>();
        private Vector2 _scrollPosition;
        private TimeSpan _totalDuration;
        void OnEnable()
        {
            _fontStyle = new GUIStyle()
            {
                richText = true,
                fontSize = 11,
                normal = new GUIStyleState() { textColor = Color.grey }
            };

            _titleStyle = new GUIStyle()
            {
                richText = true,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                border = new RectOffset(0, 0, 5, 5),
                normal = new GUIStyleState() { textColor = Color.white * 0.7f }
            };
        }
        void OnGUI()
        {
            UpdateDrawInfo();
            Draw();
        }
        void UpdateDrawInfo()
        {
            if (BuildMechine.Instance != null)
            {
                _buildInfos.Clear();
                for (var index = 0; index < BuildMechine.Instance.Actions.Count; index++)
                {
                    var instanceAction = BuildMechine.Instance.Actions[index];
                    var state = "<color=grey>wait</color>";

                    if (index == BuildMechine.Instance.CurrentActionIndex)
                    {
                        if (BuildMechine.Instance.AnyError == false)
                        {
                            state = "<color=yellow>running</color>";
                        }
                        else
                        {
                            state = "<color=red>fail</color>";
                        }
                    }
                    else if (index < BuildMechine.Instance.CurrentActionIndex)
                    {
                        state = "<color=green>done</color>";
                    }

                    _buildInfos.Add(new BuildInfo()
                    {
                        Index = index,
                        ActionName = instanceAction.GetType().Name,
                        State = state,
                        Duration = BuildMechine.Instance.Actions[index].Timer.Duration
                    });
                }
                _totalDuration = BuildMechine.Instance.MechineTimer.Duration;

                BuildMechine.ShowProgress();
            }
        }
        private void Draw()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(string.Format("Total Time : {0}:{1:d2}.{2:d3}",
                _totalDuration.Minutes,
                _totalDuration.Seconds,
                _totalDuration.Milliseconds), _titleStyle);

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                //-----------------------------BuildInfos-----------------------------//
                EditorGUILayout.LabelField("BuildInfos", _titleStyle);

                foreach (var info in _buildInfos)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    {
                        EditorGUILayout.LabelField((info.Index + 1).ToString("d2") + ".", _fontStyle, GUILayout.Width(30));
                        EditorGUILayout.LabelField(string.Format("[{0}", info.ActionName), _fontStyle, GUILayout.Width(300));
                        EditorGUILayout.LabelField(string.Format("] : "), _fontStyle, GUILayout.Width(25));
                        EditorGUILayout.LabelField(string.Format("[{0}]", info.State), _fontStyle, GUILayout.Width(50));

                        EditorGUILayout.LabelField(string.Format("{0:d3}.{1:d3}",
                                (int)info.Duration.TotalSeconds,
                                info.Duration.Milliseconds),
                            GUILayout.Width(80));
                    }
                    EditorGUILayout.EndHorizontal();
                }


            }
            EditorGUILayout.EndScrollView();
        }
        void Update()
        {
            if (BuildMechine.Instance != null)
            {
                BuildMechine.Instance.UpdateMethod();
            }
            Repaint();
        }
    }
}