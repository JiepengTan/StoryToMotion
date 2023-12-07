using System;
using System.Collections;
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

        public void Awake()
        {
            Instance = this;
            if (recorder == null)
            {
                recorder = GetComponent<Recorder>();
            }
            playableDirector = GetComponent<PlayableDirector>();
            if (playableDirector)
                playableDirector.enabled = false;


            recorder.recordingName = SaveFileName;
            recorder.disableIfNotPlaying = false;
            recorder.DoAwake();
            recorder.StartRecording();
        }

        public void ProcessData(IDataFrame frame)
        {
            DataFrame jsonFrame = frame as DataFrame;
            var frameData = jsonFrame.ParseFromJson<FrameData>();
            frameData.Time = frame.Time;
            worldContext.Deserialize(frameData);
        }

        private void SaveData()
        {
            if (worldContext == null) return;
            var frameData = worldContext.Serialize();
            recorder.RecordAsJson(frameData);
        }


        public void OnTaskDone(AIAgent agent)
        {
            recorder.SaveRecording();
            EditorApplication.isPlaying = false;
        }


        void Parse()
        {
        }

        public void LateUpdate()
        {
            SaveData();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}