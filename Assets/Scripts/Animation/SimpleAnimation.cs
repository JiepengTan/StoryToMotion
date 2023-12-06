using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace RealDream.Animation
{
    public class SimpleAnimation : BasicAnimation
    {
        AnimationClipPlayable m_mixerPlayable;
        PlayableGraph m_playableGraph;
        private Playable playable;
        private float animLength = 1;
   
        public override void SetPlayTime(float time)
        {
            if(animLength!= 0)
                time = time % animLength;
            m_mixerPlayable.SetTime(time);
            m_mixerPlayable.Play();
            m_playableGraph.Evaluate(0.01f);
        }
        public override void UpdateClip(AnimationClip clip1, AnimationClip clip2, float weight)
        {
            m_mixerPlayable = AnimationClipPlayable.Create(m_playableGraph, clip1);
            m_mixerPlayable.SetSpeed( clip1.length);
            animLength = clip1.length;
            playable.DisconnectInput(0);
            playable.ConnectInput(0, m_mixerPlayable, 0);
            m_mixerPlayable.Play();
        }
        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            m_playableGraph = playable.GetGraph();
            playable.ConnectInput(0, m_mixerPlayable, 0);
            this.playable = playable;
        }

    }
}