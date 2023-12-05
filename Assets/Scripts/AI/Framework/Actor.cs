using System;
using UnityEngine;

namespace RealDream.AI
{
    public class Actor : MonoBehaviour
    {
        [NonSerialized] public int Id;

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