using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RealDream.AI;
using RealDream.Replay;
using RecordAndRepeat;
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

        public class TransformTrack
        {
            public struct TransformClip
            {
                public List<TransformData> Data;
                public float Start;
                public float End;
            }

            public List<TransformClip> Clips = new List<TransformClip>();
        }

        public class ActorData
        {
            public int InstanceId;
            public int AssetId;
            public List<AgentData> Datas = new List<AgentData>();
            public AnimTrack Animation = new AnimTrack();
            public TransformTrack Transform = new TransformTrack();

            public void Parse()
            {
                if (Datas.Count == 0) 
                    return;
                ExtraAnim();
            }

            private void ExtraAnim()
            {
                var lastName = "";
                AnimTrack.ClipInfo clipInfo = new AnimTrack.ClipInfo();
                foreach (var data in Datas)
                {
                    var name = data.Animation.Anim1.Name;
                    if (name != lastName)
                    {
                        if (!string.IsNullOrEmpty(lastName))
                        {
                            clipInfo.End = data.Time;
                            Animation.Clips.Add(clipInfo);
                        }

                        clipInfo = new AnimTrack.ClipInfo()
                        {
                            Data = lastName,
                            Start = data.Time
                        };
                        lastName = name;
                    }
                }

                clipInfo.End = Datas[Datas.Count - 1].Time;
                Animation.Clips.Add(clipInfo);
            }
        }

        static List<ActorData> ToActorData(List<IDataFrame> frames)
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
                    dict.TryAdd(agent.InstanceId, new ActorData() { InstanceId = agent.InstanceId,AssetId = agent.AssetId});
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

        public static void Convert(RecordingBase config)
        {
            var frames = config.DataFrames;
            var timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            var actorDatas = ToActorData(frames);
            {
                var track = timelineAsset.CreateTrack<RecordingTrack>();
                var timelineClip = track.CreateClip<RecordingClip>();
                var clip = timelineClip.asset as RecordingClip;
                clip.template.recording = config;
                clip.name = config.name;
            }
            foreach (var data in actorDatas)
            {
                var track = timelineAsset.CreateTrack<AnimationTrack>($"{data.InstanceId}_anim");
                foreach (var clip in data.Animation.Clips)
                {
                    var prefab = ResourceManager.Instance.LoadPrefab(data.AssetId);
                    if(prefab == null) continue;
                    var anim = prefab.GetComponent<PlayableAnimator>();
                    var animClip = anim?.FindClip(clip.Data);
                    if(animClip == null) continue;
                    var timelineClip = track.CreateClip(animClip);
                    timelineClip.start = clip.Start;
                    timelineClip.duration = clip.End - clip.Start;
                }
            }
            SaveConfig(timelineAsset);
        }

        static void SaveConfig(TimelineAsset asset)
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