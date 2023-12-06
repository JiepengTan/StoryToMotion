using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace RealDream.Animation
{
    public class BasicAnimation:PlayableBehaviour
    {
        public float weight;
   
        public virtual void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
        }
        public virtual void SetPlayTime(float time)
        {
        }
    }

    public class BlendAnimation : BasicAnimation
    {
        AnimationMixerPlayable m_mixerPlayable;
        Playable m_fatherMixerPlayable; //控制所有AnimationBlendPlayableBehaviour的AnimationMixerPlayable
        PlayableGraph m_playableGraph;
        float m_firstClipLength, m_secondClipLength;

    
        public override void SetPlayTime(float time)
        {
            if(m_firstClipLength!= 0)
                time = time % m_firstClipLength;
            m_mixerPlayable.SetTime(time);
            UpdateWight();
            m_mixerPlayable.Play();
            m_playableGraph.Evaluate(0.01f);
            
        }
        public override void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
            var clip1Playable = AnimationClipPlayable.Create(m_playableGraph, clip1);
            var clip2Playable = AnimationClipPlayable.Create(m_playableGraph, clip2);
            m_mixerPlayable.DisconnectInput(0);
            m_mixerPlayable.DisconnectInput(1);
            m_mixerPlayable.ConnectInput(0, clip1Playable, 0);
            m_mixerPlayable.ConnectInput(1, clip2Playable, 0);
            base.weight = Mathf.Clamp01(weight);
            m_firstClipLength = clip1.length;
            m_secondClipLength = clip2.length;
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
            UpdateWight();
        }

        private void UpdateWight()
        {
            float secondClipWeight = 1.0f - weight;
            m_mixerPlayable.SetInputWeight(0, weight);
            m_mixerPlayable.SetInputWeight(1, secondClipWeight);
            float mixerPlayableSpeed = 1.0f / (weight * m_firstClipLength + secondClipWeight * m_secondClipLength);
      
        }
    }
}