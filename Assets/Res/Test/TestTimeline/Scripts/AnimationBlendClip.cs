using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Test
{
    [CreateAssetMenu(menuName = "Assets/Anim/BlendClip")]
    public class AnimationBlendClip : PlayableAsset
    {
        public AnimationClip firstClip;
        public AnimationClip secondClip;
        [Range(0, 1)] public float firstClipWeight;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var scriptPlayable = ScriptPlayable<AnimationBlendPlayableBehaviour>.Create(graph, 1);
            var animationBlendPlayable = scriptPlayable.GetBehaviour();
            animationBlendPlayable.Init(firstClip, secondClip, firstClipWeight);
            return scriptPlayable;
        }
    }
}