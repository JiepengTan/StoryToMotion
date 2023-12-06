using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace RealDream.Animation
{
    public class BasicAnimation:PlayableBehaviour
    {
        public float firstClipWeight;
        public virtual void UpdateClip(AnimationClip clip1)
        {
        }
        public virtual void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
        }
        public virtual void SetPlayTime(float time)
        {
        }
    }

    public class SimpleAnimation : BasicAnimation
    {
        AnimationClipPlayable m_mixerPlayable;
        PlayableGraph m_playableGraph;
        private Playable playable;
        public override void UpdateClip(AnimationClip clip1)
        {
            UpdateClip(clip1, clip1, 1);
        }
        public override void SetPlayTime(float time)
        {
            m_mixerPlayable.SetTime(time);
            m_mixerPlayable.Play();
            m_playableGraph.Evaluate(0.01f);
        }
        public override void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
            m_mixerPlayable = AnimationClipPlayable.Create(m_playableGraph, clip1);
            m_mixerPlayable.SetSpeed( clip1.length);
            playable.DisconnectInput(0);
            playable.ConnectInput(0, m_mixerPlayable, 0);
        }
        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            m_playableGraph = playable.GetGraph();
            playable.ConnectInput(0, m_mixerPlayable, 0);
            this.playable = playable;
        }

    }
    public class BlendAnimation : BasicAnimation
    {
        AnimationMixerPlayable m_mixerPlayable;
        Playable m_fatherMixerPlayable; //控制所有AnimationBlendPlayableBehaviour的AnimationMixerPlayable
        PlayableGraph m_playableGraph;
        float m_firstClipLength, m_secondClipLength;

        public override void UpdateClip(AnimationClip clip1)
        {
            UpdateClip(clip1, clip1, 1);
        }
        public override void SetPlayTime(float time)
        {
            m_mixerPlayable.SetTime(time);
            m_mixerPlayable.Play();
        }
        public override void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
            var clip1Playable = AnimationClipPlayable.Create(m_playableGraph, clip1);
            var clip2Playable = AnimationClipPlayable.Create(m_playableGraph, clip2);
            m_mixerPlayable.ConnectInput(0, clip1Playable, 0);
            m_mixerPlayable.ConnectInput(1, clip2Playable, 0);
            firstClipWeight = Mathf.Clamp01(weight);
            m_firstClipLength = clip1.length;
            m_secondClipLength = clip2.length;
            clip1Playable.SetSpeed(m_firstClipLength);
            clip2Playable.SetSpeed(m_secondClipLength);
        }

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);

            m_playableGraph = playable.GetGraph();
            m_mixerPlayable = AnimationMixerPlayable.Create(m_playableGraph, 2);
            playable.ConnectInput(0, m_mixerPlayable, 0);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            float secondClipWeight = 1.0f - firstClipWeight;
            m_mixerPlayable.SetInputWeight(0, firstClipWeight);
            m_mixerPlayable.SetInputWeight(1, secondClipWeight);
            float mixerPlayableSpeed = 1.0f / (firstClipWeight * m_firstClipLength + secondClipWeight * m_secondClipLength);
            m_mixerPlayable.SetSpeed(mixerPlayableSpeed);
        }


    }
}