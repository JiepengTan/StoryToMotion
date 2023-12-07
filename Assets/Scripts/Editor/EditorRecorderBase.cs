using System;
using RecordAndRepeat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace RealDream.EditorExt
{
    [CustomEditor(typeof(ReplayManager))]
    public class EditorReplayManager : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            DrawLoad();

            DrawPreview();
            GUILayout.EndHorizontal();
        }

        private void DrawLoad()
        {
            if (GUILayout.Button("Load replay file "))
            {
                var owner = target as ReplayManager;
                TimelineConvert.OutputFileName = owner.SaveFileName;
                var data = AssetDatabase.LoadAssetAtPath<RecordingBase>(TimelineConvert.OutputPath + ".asset");
                TimelineConvert.Convert(data);
                var timelineData = AssetDatabase.LoadAssetAtPath<TimelineAsset>(TimelineConvert.OutputPath + ".playable");
                owner.playableDirector.playableAsset = timelineData;
            }
        }
        private void DrawPreview()
        {
            if (GUILayout.Button("Preload"))
            {
                var owner = target as ReplayManager;
                TimelineConvert.OutputFileName = owner.SaveFileName;
                var data = AssetDatabase.LoadAssetAtPath<RecordingBase>(TimelineConvert.OutputPath + ".asset");
                TimelineConvert.Convert(data);
                var timelineData = AssetDatabase.LoadAssetAtPath<TimelineAsset>(TimelineConvert.OutputPath + ".playable");
                owner.playableDirector.playableAsset = timelineData;
                
                
                //director.SetGenericBinding(track1, Animators);
               // director.SetGenericBinding(track2, Animators2);
            }
        }
    }

    [CustomEditor(typeof(Recording))]
    public class EditorRecording : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            serializedObject.Update();

            RecordingBase recording = target as RecordingBase;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // recording details as labels
            EditorGUILayout.LabelField("Recording Name", recording.name);
            EditorGUILayout.LabelField("Type",recording.GetType().Name);
            EditorGUILayout.LabelField("Duration", String.Format("{0:N2}", recording.duration));
            EditorGUILayout.LabelField("Count", recording.Count().ToString());

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            // button to log all data records
            if (GUILayout.Button("Log Recording", EditorStyles.miniButtonMid, GUILayout.Height(20)))
            {
                recording.Log();
            }
        
            EditorGUILayout.Space();

            // show data fields
            SerializedProperty field = serializedObject.GetIterator();
            field.NextVisible(true);
            while (field.NextVisible(false))
            {
                EditorGUILayout.PropertyField(field, true);
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            
            EditorGUILayout.Space();
        }
    }
}