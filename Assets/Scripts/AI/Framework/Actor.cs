using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RealDream.AI
{
    public class Actor : MonoBehaviour
    {
        public int AssetId;
        [ReadOnly] public int InstanceId;

        // TODO use int to replace string
        public string[] Tags;

        public bool HasTag(string tag)
        {
            foreach (var item in Tags)
            {
                if (item == tag) return true;
            }

            return false;
        }

        private ActorComponent[] components;

        void Awake()
        {
            var assetRef = GetComponent<AssetRef>();
            if (assetRef != null)
                AssetId = assetRef.AssetId;
            components = GetComponentsInChildren<ActorComponent>();
            if (WorldContext.IsReplayMode) 
                return;
            WorldContext.Instance.AddActor(this);
        }

        public void DoAwake()
        {
            OnAwake();
            foreach (var comp in components)
            {
                comp.DoAwake();
            }
        }

        public void DoStart()
        {
            OnStart();
            foreach (var comp in components)
            {
                comp.DoStart();
            }
        }


        public void DoUpdate(float dt)
        {
            OnUpdate(dt);
            foreach (var comp in components)
            {
                comp.DoUpdate(dt);
            }
        }

        public virtual void DoDestroy()
        {
            foreach (var comp in components)
            {
                comp.DoDestroy();
            }

            _OnDestroy();
        }

        public void DestroySelf()
        {
            if (WorldContext.Instance != null)
                WorldContext.Instance.RemoveActor(this);
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate(float dt)
        {
        }

        protected virtual void _OnDestroy()
        {
        }
    }
}