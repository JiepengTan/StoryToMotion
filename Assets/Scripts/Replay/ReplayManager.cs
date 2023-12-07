using System;
using System.Collections;
using System.Collections.Generic;
using RealDream.AI;
using RecordAndRepeat;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using FrameData = RealDream.Replay.FrameData;

namespace RealDream
{
    [RequireComponent(typeof(Recorder))]
    [RequireComponent(typeof(DataListener))]
    [RequireComponent(typeof(PlayableDirector))]
    public class ReplayManager : MonoBehaviour, IDataReceived
    {
        public static ReplayManager Instance;
        private Recorder recorder;
        public string SaveFileName;
        public WorldContext worldContext;

        public PlayableDirector playableDirector;
        public Transform replayRoot;

        public bool IsReplayMode = false;
        public void DoAwake()
        {
            Instance = this;
            if (recorder == null)
            {
                recorder = GetComponent<Recorder>();
            }
            playableDirector = GetComponent<PlayableDirector>();
            if (IsReplayMode)
            {
                var asset = ScriptableObject.Instantiate(playableDirector.playableAsset);
                playableDirector.playableAsset = asset;
                Dictionary<int, AIAgent> id2Agents = InitEditorAgentMap();
                RebindTimeline(playableDirector, id2Agents);
                playableDirector.Play();
                return;
            }
           
            if (replayRoot != null)
                GameObject.DestroyImmediate(replayRoot.gameObject);

            if (playableDirector)
                playableDirector.enabled = false;
            recorder.recordingName = SaveFileName;
            recorder.disableIfNotPlaying = false;
            recorder.DoAwake();
            recorder.StartRecording();
        }
        public void DoLateUpdate()
        {
            SaveData();
        }

        public void ProcessData(IDataFrame frame)
        {
            DataFrame jsonFrame = frame as DataFrame;
            var frameData = jsonFrame.ParseFromJson<FrameData>();
            frameData.Time = frame.Time;
            Deserialize(frameData);
        }

        private void SaveData()
        {
            if (worldContext == null) return;
            var frameData = worldContext.Serialize();
            recorder.RecordAsJson(frameData);
        }

        public void Deserialize(FrameData frameData)
        {
            if (Application.isPlaying && !IsReplayMode ) return;
            //CreateReplayRoot();
            if(replayRoot == null) return;
            var id2Agent = InitEditorAgentMap();
            foreach (var data in frameData.Agents)
            {
                if (!id2Agent.ContainsKey(data.InstanceId))
                    continue;
                var agent = id2Agent[data.InstanceId];
                agent.transform.position = data.Transform.Pos;
                agent.transform.eulerAngles = data.Transform.Rot;
                agent.transform.localScale = data.Transform.Scale;
            }
        }

        public Dictionary<int, AIAgent> InitEditorAgentMap()
        {
            Dictionary<int, AIAgent> id2Agent = new Dictionary<int, AIAgent>();
            var childCount = replayRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var tran = replayRoot.GetChild(i);
                var agent = tran.GetComponent<AIAgent>();
                if (agent != null)
                {
                    id2Agent[agent.InstanceId] = agent;
                }
            }

            return id2Agent;
        }
        public bool RebindTimeline(PlayableDirector director, Dictionary<int, AIAgent> id2Agents)
        {
            var timeline = director.playableAsset as TimelineAsset;
            var tracks = timeline.GetOutputTracks();
            Dictionary<string, TrackAsset> name2Track = new Dictionary<string, TrackAsset>();
            foreach (var track in tracks)
            {
                name2Track[track.name] = track;
            }

            if (!name2Track.ContainsKey(TimelineConvert.TopTrackName))
            {
                Debug.LogError("Data error: miss root track");
                return true;
            }

            director.SetGenericBinding(name2Track[TimelineConvert.TopTrackName], GetComponent<DataListener>());
            foreach (var pair in id2Agents)
            {
                var agent = pair.Value;
                var animTrack = TimelineConvert.GetTrackName(agent.InstanceId, TimelineConvert.ETrackType.Anim);
                var activeTrack = TimelineConvert.GetTrackName(agent.InstanceId, TimelineConvert.ETrackType.Active);
                director.SetGenericBinding(name2Track[activeTrack], agent.gameObject);
                director.SetGenericBinding(name2Track[animTrack], agent.GetComponentInChildren<Animator>());
            }

            return false;
        }
        public void CreateReplayRoot()
        {
            if (replayRoot != null)
            {
                var lst = new List<Transform>();
                var childCount = replayRoot.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    lst.Add(replayRoot.GetChild(i));
                }

                foreach (var tran in lst)
                {
                    EditorPool.DestroyAgent(tran.gameObject);
                }
            }

            if (replayRoot == null)
                replayRoot = new GameObject("__replayRoot").transform;
            replayRoot.SetParent(transform, true);
        }

        private int finishedCount = 0;
        
        public void OnTaskDone(AIAgent agent)
        {
            if(IsReplayMode) return;
            finishedCount++;
            if (finishedCount == WorldContext.Instance.AgentCount)
            {
                recorder.SaveRecording();
                EditorApplication.isPlaying = false;
            }
        }
        public void OnDestroy()
        {
            Instance = null;
        }
    }
}