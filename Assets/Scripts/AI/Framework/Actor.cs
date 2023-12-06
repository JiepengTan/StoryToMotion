using System;
using UnityEngine;

namespace RealDream.AI
{
    public class Actor : MonoBehaviour
    {
        public int AssetId;
        [NonSerialized] public int InstanceId;

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

        void Awake()
        {
            var assetRef = GetComponent<AssetRef>();
            if (assetRef != null)
                AssetId = assetRef.AssetId;
            WorldContext.Instance.AddActor(this);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        void OnDestroy()
        {
            if (WorldContext.Instance != null)
                WorldContext.Instance.RemoveActor(this);
        }
    }
}