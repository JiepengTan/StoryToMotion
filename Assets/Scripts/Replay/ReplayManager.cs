using System;
using System.Collections;
using RealDream.AI;
using RealDream.Replay;
using RecordAndRepeat;
using UnityEditor;
using UnityEngine;

namespace RealDream
{
    [RequireComponent(typeof(Recorder))]
    [RequireComponent(typeof(DataListener))]
    public class ReplayManager : MonoBehaviour,IDataReceived
    {
        public static ReplayManager Instance;
        private Recorder recorder;
        public string SaveFileName;
        public WorldContext worldContext;

        private DataListener dataListener;
        public void Awake()
        {
            Instance = this;
            if (recorder == null)
            {
                recorder = GetComponent<Recorder>();
            }
            dataListener = GetComponent<DataListener>();
            
            recorder.recordingName = SaveFileName;
            recorder.disableIfNotPlaying = false;
            recorder.DoAwake();
            recorder.StartRecording();
        }

        public void ProcessData(IDataFrame frame)
        {
            DataFrame jsonFrame = frame as DataFrame;
            var frameData = jsonFrame.ParseFromJson<FrameData>();
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