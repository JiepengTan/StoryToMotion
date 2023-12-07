using System.Collections.Generic;
using RealDream.AI;
using RecordAndRepeat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RealDream.EditorExt
{
    [CustomEditor(typeof(ReplayManager))]
    public class EditorReplayManager : Editor
    {
        private ReplayManager owner;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            owner = target as ReplayManager;

            GUILayout.BeginHorizontal();
            DrawLoad();
            DrawPreview();
            DrawClear();
            GUILayout.EndHorizontal();
        }

        private void DrawClear()
        {
            if (GUILayout.Button("Clear"))
            {
                GameObject.DestroyImmediate(owner.replayRoot.gameObject);
                owner.playableDirector.playableAsset = null;
            }
        }

        private void DrawLoad()
        {
            if (GUILayout.Button("Load"))
            {
                TimelineConvert.OutputFileName = owner.SaveFileName;
                var data = AssetDatabase.LoadAssetAtPath<RecordingBase>(TimelineConvert.OutputPath + ".asset");
                TimelineConvert.Convert(data);
                var timelineData =
                    AssetDatabase.LoadAssetAtPath<TimelineAsset>(TimelineConvert.OutputPath + ".playable");
                owner.playableDirector.playableAsset = timelineData;
            }
        }

        private void DrawPreview()
        {
            if (GUILayout.Button("Show"))
            {
                owner.CreateReplayRoot();
                TimelineConvert.OutputFileName = owner.SaveFileName;
                var config = AssetDatabase.LoadAssetAtPath<RecordingBase>(TimelineConvert.OutputPath + ".asset");
                var actorDatas = TimelineConvert.ToActorData(config.DataFrames);
                CreateInstance(actorDatas);
                Dictionary<int, AIAgent> id2Agents = owner.InitEditorAgentMap();
                owner.RebindTimeline(owner.playableDirector, id2Agents);
            }
        }


        private void CreateInstance(List<TimelineConvert.ActorData> actorDatas)
        {
            foreach (var data in actorDatas)
            {
                var assetId = data.AssetId;
                var go = EditorPool.CreateAgent(assetId);
                if (go == null)
                    continue;
                var agent = go.GetComponent<AIAgent>();
                agent.InstanceId = data.InstanceId;
                go.transform.SetParent(owner.replayRoot, false);
                go.SetActive(false);
            }
        }
    }
}