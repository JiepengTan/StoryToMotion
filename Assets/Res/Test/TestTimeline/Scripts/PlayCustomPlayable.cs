using RealDream.Animation;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Test
{
    public class PlayCustomPlayable : MonoBehaviour
    {
        public Animator Anim;

        [Range(0, 1)] public float weight;
        public AnimationClip clip1;
        public AnimationClip clip2;

        PlayableGraph m_graph;
        BlendAnimation m_blendPlayableBehaviour;

        void Start()
        {
            if (Anim == null)
            {
                Anim = GetComponentInChildren<Animator>();
            }

            m_graph = PlayableGraph.Create("ChanPlayableGraph");
            var animationOutputPlayable = AnimationPlayableOutput.Create(m_graph, "AnimationOutput", Anim);
            var blendPlayable = ScriptPlayable<BlendAnimation>.Create(m_graph, 1);
            m_blendPlayableBehaviour = blendPlayable.GetBehaviour();
            m_blendPlayableBehaviour.UpdateClip(clip1, clip2, 1);
            animationOutputPlayable.SetSourcePlayable(blendPlayable);
            m_graph.Play();
        }

        private void Update()
        {
            m_blendPlayableBehaviour.weight = weight;
        }

        void OnDestroy()
        {
            m_graph.Destroy();
        }
    }
}