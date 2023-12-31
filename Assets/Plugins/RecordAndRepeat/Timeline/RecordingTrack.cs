using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RecordAndRepeat
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(RecordingClip))]
    [TrackBindingType(typeof(DataListener))]
    public class RecordingTrack : TrackAsset
    {
        public TimelineClip CreateClip(RecordingBase clip)
        {
            if (clip == null)
                return null;

            var newClip = CreateClip<RecordingClip>();
            RecordingClip asset = newClip.asset as RecordingClip;
            if (asset != null)
            {
                asset.template.recording = clip;
            }

            return newClip;
        }
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                RecordingClip recordingClip = clip.asset as RecordingClip;
                RecordingBase recordingRef = recordingClip.template.recording;
                if (recordingRef)
                {
                    if (recordingClip.template.RecordingChanged())
                    {
                        clip.displayName = recordingRef.name;
                        clip.duration = recordingClip.duration;
                        clip.clipIn = 0;
                    }
                }
                else
                {
                    clip.displayName = "...";
                }
            }

            return ScriptPlayable<RecordingMixerBehaviour>.Create(graph, inputCount);
        }
    }
}