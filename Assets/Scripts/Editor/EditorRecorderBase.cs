using System;
using RecordAndRepeat;
using UnityEditor;
using UnityEngine;

namespace RealDream.EditorExt
{
    [CustomEditor(typeof(Recording))]
    public class EditorRecording : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            RecordingBase recording = target as RecordingBase;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // recording details as labels
            EditorGUILayout.LabelField("Recording Name", recording.name);
            EditorGUILayout.LabelField("Type", recording.GetType().Name);
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