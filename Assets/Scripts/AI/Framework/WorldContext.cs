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

        public Transform replayRoot;

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
                frameData.Agents.Add(data);
            }

            frameData.CurId = _id;
            return frameData;
        }

        public void Deserialize(FrameData frameData)
        {
            if(Application.isPlaying) return;
            if (replayRoot != null)
            {
                var lst = new List<Transform>();
                var childCount = replayRoot.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    lst.Add(replayRoot.GetChild(i));
                }

                foreach (var tran in lst)
                {
                   EditorPool.DestroyAgent(tran.gameObject);
                }
            }
            if(replayRoot == null)
                replayRoot = new GameObject("__replayRoot").transform;
            replayRoot.SetParent(transform, true);

            _id = frameData.CurId;

            foreach (var data in frameData.Agents)
            {
                var assetId = data.AssetId;
                var go = EditorPool.CreateAgent(assetId);
                if(go == null)
                    continue;
                go.transform.SetParent(replayRoot,false);
                var agent =go.GetComponent<AIAgent>();
                agent.InstanceId = data.InstanceId;
                agent.transform.position = data.Transform.Pos;
                agent.transform.eulerAngles = data.Transform.Rot;
                agent.transform.localScale = data.Transform.Scale;
            }
        }


        void Awake()
        {
            _id = 1;
            Instance = this;
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