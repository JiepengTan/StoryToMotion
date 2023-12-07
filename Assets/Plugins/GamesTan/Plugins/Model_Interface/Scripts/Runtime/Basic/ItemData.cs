using System;
using UnityEngine;

namespace RealDream {
    [System.Serializable]
    public class BaseItemData {
        public long EntityId;
        public int Id;
        [Tooltip("数量")] public long Count = 1;

        public override string ToString() {
            return $"EntityId={EntityId} Id={Id} ";
        }
    }
}