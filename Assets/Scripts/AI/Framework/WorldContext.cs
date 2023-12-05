using System.Collections;
using System.Collections.Generic;
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
            actor.Id = _id++;
            _id2Actor[actor.Id] = actor;
            foreach (var tag in actor.Tags)
            {
                if (!_tag2Objects.ContainsKey(tag))
                {
                    _tag2Objects[tag] = new HashSet<int>();
                }

                _tag2Objects[tag].Add(actor.Id);
            }
        }

        public void RemoveActor(Actor actor)
        {
            _id2Actor.Remove(actor.Id);
            foreach (var tag in actor.Tags)
            {
                if (_tag2Objects.ContainsKey(tag))
                {
                    _tag2Objects[tag].Remove(actor.Id);
                    if (_tag2Objects[tag].Count == 0)
                    {
                        _tag2Objects.Remove(tag);
                    }
                }
            }
        }


        void Awake()
        {
            _id = 1;
            Instance = this;
        }

        public void Update()
        {
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