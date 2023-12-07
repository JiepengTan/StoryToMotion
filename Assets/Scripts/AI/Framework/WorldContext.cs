using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RealDream.Replay;
using UnityEngine;


namespace RealDream.AI
{
    public class WorldContext : MonoBehaviour
    {
        private static int _id = 0;
        public static WorldContext Instance { get; private set; }
        private Dictionary<string, UData> _states = new Dictionary<string, UData>();
        private Dictionary<string, HashSet<int>> _tag2Objects = new Dictionary<string, HashSet<int>>();
        private Dictionary<int, Actor> _id2Actor = new Dictionary<int, Actor>();

        [HideInInspector]  public Transform replayRoot;

        public int AgentCount => _id2Actor.Values.Where(a => a is AIAgent).Count();

        private ReplayManager replayManager;
        private bool _hasStart;

        private Queue<Actor> _pendingInitActors = new Queue<Actor>();

        void Awake()
        {
            _id = 1;
            Instance = this;
            AssetsUtil.SetLoader(new GameAssetLoader());
            if (replayManager == null)
                replayManager = FindObjectOfType<ReplayManager>();
            _hasStart = false;
            replayManager?.DoAwake();
        }


        void Start()
        {
            if (replayManager != null && replayManager.IsReplayMode)
            {
                return;
            }

            var actors = _pendingInitActors.ToArray();
            _pendingInitActors.Clear();
            foreach (var actor in actors)
            {
                if (actor != null)
                    actor.DoAwake();
            }

            foreach (var actor in actors)
            {
                if (actor != null)
                    actor.DoStart();
            }

            // second batch
            while (_pendingInitActors.Count > 0)
            {
                var actor = _pendingInitActors.Dequeue();
                if (actor != null)
                {
                    actor.DoAwake();
                    if (actor != null) actor.DoStart();
                }
            }

            _hasStart = true;
        }

        private void Update()
        {
            if(replayManager!= null &&  replayManager.IsReplayMode) 
                return;
            var actors = _id2Actor.Values.ToArray();
            float dt = Time.deltaTime;
            foreach (var actor in actors)
            {
                if (actor != null)
                {
                    actor.DoUpdate(dt);
                }
            }

            replayManager.DoLateUpdate();
        }


        public FrameData Serialize()
        {
            var frameData = new FrameData();
            var lst = new List<AIAgent>();
            foreach (var item in _id2Actor)
            {
                var agent = item.Value as AIAgent;
                if (agent != null)
                {
                    lst.Add(agent);
                }
            }

            foreach (var agent in lst)
            {
                var data = new AgentData();
                var transData = new TransformData();
                transData.Pos = agent.transform.position;
                transData.Rot = agent.transform.eulerAngles;
                transData.Scale = agent.transform.lossyScale;
                data.Transform = transData;
                data.AssetId = agent.AssetId;
                data.InstanceId = agent.InstanceId;
                data.Animation = agent.anim.GetAnimInfo();
                frameData.Agents.Add(data);
            }

            frameData.CurId = _id;
            return frameData;
        }


        public List<Actor> GetActors(string tag)
        {
            var result = new List<Actor>();
            if (_tag2Objects.ContainsKey(tag))
            {
                foreach (var item in _tag2Objects[tag])
                {
                    var actor = _id2Actor[item];
                    if (actor != null)
                    {
                        result.Add(actor);
                    }
                }
            }

            return result;
        }

        public void SetState(string key, UData value)
        {
            _states[key] = value;
        }

        public UData GetState(string key)
        {
            if (_states.ContainsKey(key))
            {
                return _states[key];
            }

            return null;
        }

        public void AddActor(Actor actor)
        {
            actor.InstanceId = _id++;
            _id2Actor[actor.InstanceId] = actor;
            foreach (var tag in actor.Tags)
            {
                if (!_tag2Objects.ContainsKey(tag))
                {
                    _tag2Objects[tag] = new HashSet<int>();
                }

                _tag2Objects[tag].Add(actor.InstanceId);
            }

            if (_hasStart)
            {
                actor.DoAwake();
                actor.DoStart();
            }
            else
            {
                _pendingInitActors.Enqueue(actor);
            }
        }

        public void RemoveActor(Actor actor)
        {
            _id2Actor.Remove(actor.InstanceId);
            foreach (var tag in actor.Tags)
            {
                if (_tag2Objects.ContainsKey(tag))
                {
                    _tag2Objects[tag].Remove(actor.InstanceId);
                    if (_tag2Objects[tag].Count == 0)
                    {
                        _tag2Objects.Remove(tag);
                    }
                }
            }
            actor.DoDestroy();
        }

        public void OnDestroy()
        {
            Instance = null;
            _states.Clear();
            _tag2Objects.Clear();
            _id2Actor.Clear();
        }
    }
}