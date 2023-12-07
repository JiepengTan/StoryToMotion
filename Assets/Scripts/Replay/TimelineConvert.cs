using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RealDream.AI;
using RealDream.Replay;
using RecordAndRepeat;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace RealDream
{
    public class TimelineConvert
    {
        public static string OutputPath => Path.Combine(OutputDir, OutputFileName);
        public static string OutputDir => RecorderBase.RecordingsPath;
        public static string OutputFileName = "OutputWorld";
        public static string TopTrackName = "0_All";


        public class AnimTrack
        {
            public struct ClipInfo
            {
                public string Data;
                public float Start;
                public float End;
            }

            public List<ClipInfo> Clips = new List<ClipInfo>();
        }

        public class ActiveTrack
        {
            public struct ClipInfo
            {
                public bool Data;
                public float Start;
                public float End;
            }

            public List<ClipInfo> Clips = new List<ClipInfo>();
        }

        public class TransformTrack
        {
            public struct ClipInfo
            {
                public List<TransformData> Data;
                public float Start;
                public float End;
            }

            public List<ClipInfo> Clips = new List<ClipInfo>();
        }

        public class ActorData
        {
            public int InstanceId;
            public int AssetId;
            public List<AgentData> Datas = new List<AgentData>();
            public AnimTrack Animation = new AnimTrack();
            public TransformTrack Transform = new TransformTrack();
            public ActiveTrack Active = new ActiveTrack();

            public void Parse()
            {
                if (Datas.Count == 0)
                    return;
                ExtraAnim();
                ExtraActive();
            }

            private void ExtraActive()
            {
                if (Datas.Count == 0) return;
                ActiveTrack.ClipInfo clipInfo = new ActiveTrack.ClipInfo();
                clipInfo.Start = Datas[0].Time;
                clipInfo.End = Datas[1].Time;
                Active.Clips.Add(clipInfo);
            }

            private void ExtraAnim()
            {
                var lastName = "";
                AnimTrack.ClipInfo clipInfo = new AnimTrack.ClipInfo()
                {
                    Start = Datas[0].Time,
                    Data = Datas[0].Animation.Anim1.Name
                };
                foreach (var data in Datas)
                {
                    var name = data.Animation.Anim1.Name;
                    if (name != lastName)
                    {
                        clipInfo.End = data.Time;
                        Animation.Clips.Add(clipInfo);
                        clipInfo = new AnimTrack.ClipInfo()
                        {
                            Start = data.Time,
                            Data = name
                        };
                        lastName = name;
                    }
                }

                clipInfo.End = Datas[Datas.Count - 1].Time;
                Animation.Clips.Add(clipInfo);
            }
        }

        public static List<ActorData> ToActorData(List<IDataFrame> frames)
        {
            List<FrameData> frameDatas = new List<FrameData>();
            foreach (var frame in frames)
            {
                var data = (frame as DataFrame)?.ParseFromJson<FrameData>();
                data.Time = frame.Time;
                frameDatas.Add(data);
            }

            var dict = new Dictionary<int, ActorData>();
            foreach (var frame in frameDatas)
            {
                var data = frame.Agents;
                for (int i = 0; i < data.Count; i++)
                {
                    var agent = data[i];
                    dict.TryAdd(agent.InstanceId,
                        new ActorData() { InstanceId = agent.InstanceId, AssetId = agent.AssetId });
                    var actorData = dict[agent.InstanceId];
                    agent.Time = frame.Time;
                    actorData.Datas.Add(agent);
                }
            }

            foreach (var data in dict)
            {
                data.Value.Parse();
            }

            return dict.Values.ToList();
        }

        public enum ETrackType
        {
            Anim,
            Active
        }

        public static string GetTrackName(int instanceId, ETrackType type)
        {
            return $"{instanceId}_{type}";
        }

        public static void Convert(RecordingBase config)
        {
            var frames = config.DataFrames;
            var actorDatas = ToActorData(frames);
            var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            // global transform
            {
                var track = timelineAsset.CreateTrack<RecordingTrack>(TopTrackName);
                var timelineClip = track.CreateClip<RecordingClip>();
                var clip = timelineClip.asset as RecordingClip;
                clip.template.recording = config;
                clip.name = config.name;
                timelineClip.start = 0;
                timelineClip.duration = config.duration;
            }

            foreach (var data in actorDatas)
            {
                // active
                {
                    var track = timelineAsset.CreateTrack<ActivationTrack>(GetTrackName(data.InstanceId,
                        ETrackType.Active));
                    foreach (var clip in data.Animation.Clips)
                    {
                        var timelineClip = track.CreateDefaultClip();
                        timelineClip.start = clip.Start;
                        timelineClip.duration = clip.End - clip.Start;
                    }
                }
                // animation
                {
                    var track = timelineAsset.CreateTrack<AnimationTrack>(
                        GetTrackName(data.InstanceId, ETrackType.Anim));
                    foreach (var clip in data.Animation.Clips)
                    {
                        var prefab = ResourceManager.Instance.LoadPrefab(data.AssetId);
                        if (prefab == null) continue;
                        var anim = prefab.GetComponent<PlayableAnimator>();
                        var animClip = anim?.FindClip(clip.Data);
                        if (animClip == null) continue;
                        var timelineClip = track.CreateClip(animClip);
                        timelineClip.start = clip.Start;
                        timelineClip.duration = clip.End - clip.Start;
                    }
                }
            }

            SaveConfig(timelineAsset);
        }

        public static void SaveConfig(TimelineAsset asset)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(asset);

            string path = OutputDir;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(".", RecorderBase.RecordingsPath);
            }
            AssetDatabase.CreateAsset(asset, OutputPath + ".playable");
            // create timeline 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}