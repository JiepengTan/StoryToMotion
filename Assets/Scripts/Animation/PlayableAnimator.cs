using System.Collections;
using System.Collections.Generic;
using RealDream.Animation;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace RealDream
{
    [System.Serializable]
    public struct AnimationInfo
    {
        //[HorizontalGroup, LabelWidth(40)] 
        public string Name;

        //[HorizontalGroup, LabelWidth(30)] 
        public AnimationClip Clip;
    }

    public class PlayableAnimator : MonoBehaviour
    {
        public Animator Anim;

        public List<AnimationInfo> Animations = new List<AnimationInfo>();

        PlayableGraph m_graph;
        BasicAnimation proxy;

        private Dictionary<string, AnimationClip> _name2Clip = new Dictionary<string, AnimationClip>();

        public string StartAnimName = AnimName_Idle;
        private AnimationClip curAnim = null;
        private AnimationClip lastAnim = null;
        private AnimationInfo curAnimInfo;
        private float weight;
        private float crossFadeInterval;
        private float lerpTimer;

        private float _timer;
        public const string AnimName_Idle = "Idle";
        public const string AnimName_Run = "Run";

        private string curAnimName;
        private float curAnimLen;

        public Replay.AnimData GetAnimInfo()
        {
            var info = new Replay.AnimData();
            info.Anim1.Name = curAnimName;
            info.Anim1.NormalizeTime = _timer % curAnimLen;
            info.Anim1.Weight = 1;
            return info;
        }

        public void ApplyAnimInfo(Replay.AnimData info)
        {
            return;
            CheckStart();
            curAnimName = info.Anim1.Name;
            _timer = info.Anim1.NormalizeTime;
            var weight = info.Anim1.Weight;
            Play(curAnimName);
            Sample(_timer, weight);
        }

        public void Move(Vector2 dir)
        {
            if ((AnimName_Idle != curAnimName && AnimName_Run != curAnimName)
                && curAnimLen > _timer)
            {
                return;
            }

            if (dir.magnitude > 0.05f)
            {
                if(curAnimName == AnimName_Run)
                    return;
                Play(AnimName_Run);
            }
            else
            {
                if(curAnimName == AnimName_Idle)
                    return;
                Play(AnimName_Idle);
            }
        }


        public void Play(string anim, float crossFadeTime = 0.1f, bool isNeedStopMove = true)
        {
            //Debug.Log(" Play " + anim);
            curAnimName = anim;
            curAnim = FindClip(anim);
            curAnimLen = curAnim.length;
            proxy.UpdateClip(curAnim, curAnim, 0);
            crossFadeInterval = crossFadeTime;
            lerpTimer = 0;
            _timer = 0;
        }

        public bool IsSimpleMode = false;
        private bool hasInit;

        public void Start()
        {
            CheckStart();
        }

        public void CheckStart()
        {
            if(hasInit)
                return;
            hasInit = true;
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
            curAnim = FindClip(StartAnimName);
            if (curAnim == null)
            {
                curAnim = Animations[0].Clip;
            }

            if (curAnim == null)
            {
                Debug.LogError("Missing first animation");
            }

            m_graph = PlayableGraph.Create("PlayableGraph");
            var animationOutputPlayable = AnimationPlayableOutput.Create(m_graph, "AnimationOutput", Anim);

            if (IsSimpleMode)
            {
                var blendPlayable = ScriptPlayable<SimpleAnimation>.Create(m_graph, 1);
                proxy = blendPlayable.GetBehaviour();
                animationOutputPlayable.SetSourcePlayable(blendPlayable);
            }
            else
            {
                var blendPlayable = ScriptPlayable<BlendAnimation>.Create(m_graph, 1);
                proxy = blendPlayable.GetBehaviour();
                animationOutputPlayable.SetSourcePlayable(blendPlayable);
            }

            m_graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            m_graph.Play();

            Play(FindName(curAnim));
        }

        private void Update()
        {
            DoUpdate(Time.deltaTime);
        }


        private void DoUpdate(float dt)
        {
            lerpTimer += dt;
            if (crossFadeInterval == 0)
            {
                weight = 1;
            }
            else
            {
                weight = Mathf.Clamp01(lerpTimer / crossFadeInterval);
                lerpTimer %= crossFadeInterval;
            }

            _timer += dt;
            Sample(_timer, weight);
        }

        void OnDestroy()
        {
            m_graph.Destroy();
        }

        void Sample(float time, float weight = 1)
        {
            this.weight = weight;
            _timer = time;
            proxy.weight = weight;
            proxy.SetPlayTime(_timer);
        }

        string FindName(AnimationClip clip)
        {
            foreach (var info in Animations)
            {
                if (info.Clip == clip)
                    return info.Name;
            }

            return "";
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