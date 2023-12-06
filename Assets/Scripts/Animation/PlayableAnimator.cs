using System.Collections;
using System.Collections.Generic;
using RealDream.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace RealDream
{
    [System.Serializable]
    public struct AnimationInfo
    {
        [HorizontalGroup, LabelWidth(40)] public string Name;
        [HorizontalGroup, LabelWidth(30)] public AnimationClip Clip;
    }

    public class PlayableAnimator : MonoBehaviour
    {
        public Animator Anim;

        public List<AnimationInfo> Animations = new List<AnimationInfo>();

        PlayableGraph m_graph;
        BasicAnimation proxy;

        private Dictionary<string, AnimationClip> _name2Clip = new Dictionary<string, AnimationClip>();

        public AnimationClip StartAnim;
        private AnimationClip curAnim = null;
        private AnimationInfo curAnimInfo;
        private float weight;
        private float crossFadeInterval;
        private float lerpTimer;

        public Replay.AnimData GetAnimInfo()
        {
            var info = new Replay.AnimData();
            return info;
        }


        public void Move(Vector3 dir)
        {
        }

        [Range(0, 3)] [OnValueChanged(nameof(DebugOnAnimTimeChanged))]
        public float DebugAnimTime;

        public string DebugAnimName;
        public bool IsDebugSimple;

        public void DebugOnAnimTimeChanged()
        {
            if (proxy == null) return;
            proxy.SetPlayTime(DebugAnimTime);
        }

        [Button]
        public void DebugPlay()
        {
            m_graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            curAnim = FindClip(DebugAnimName);
            proxy.UpdateClip(curAnim, curAnim, 1);
        }

        void Start()
        {
            _name2Clip.Clear();
            foreach (var info in Animations)
            {
                _name2Clip[info.Name] = info.Clip;
            }

            if (Anim == null)
            {
                Anim = GetComponentInChildren<Animator>();
            }

            if (Animations.Count == 0) return;
            if (StartAnim == null)
            {
                StartAnim = Animations[0].Clip;
            }

            if (StartAnim == null)
            {
                Debug.LogError("Missing first animation");
            }

            curAnim = StartAnim;

            m_graph = PlayableGraph.Create("PlayableGraph");
            var animationOutputPlayable = AnimationPlayableOutput.Create(m_graph, "AnimationOutput", Anim);
            if (IsDebugSimple)
            {
                var blendPlayable = ScriptPlayable<SimpleAnimation>.Create(m_graph, 1);
                proxy = blendPlayable.GetBehaviour();
                proxy.UpdateClip(curAnim, curAnim, 1);
                animationOutputPlayable.SetSourcePlayable(blendPlayable);
            }
            else
            {
                var blendPlayable = ScriptPlayable<BlendAnimation>.Create(m_graph, 1);
                proxy = blendPlayable.GetBehaviour();
                proxy.UpdateClip(curAnim, curAnim, 1);
                animationOutputPlayable.SetSourcePlayable(blendPlayable);
            }

            m_graph.Play();
        }

        private void Update()
        {
            //UpdateCrossFade();
        }

        private void UpdateCrossFade()
        {
            lerpTimer += Time.deltaTime;
            if (crossFadeInterval == 0)
            {
                weight = 1;
            }
            else
            {
                weight = Mathf.Clamp01(lerpTimer / crossFadeInterval);
            }

            proxy.firstClipWeight = weight;
        }

        void OnDestroy()
        {
            m_graph.Destroy();
        }

        public void Play(string anim, float crossFadeTime = 0.2f, bool isNeedStopMove = true)
        {
            curAnim = FindClip(anim);
            proxy.UpdateClip(curAnim, curAnim, 1);
            crossFadeInterval = crossFadeTime;
            lerpTimer = 0;
        }

        AnimationClip FindClip(string name)
        {
            if (_name2Clip.TryGetValue(name, out var clip))
            {
                return clip;
            }

            return null;
        }
    }
}