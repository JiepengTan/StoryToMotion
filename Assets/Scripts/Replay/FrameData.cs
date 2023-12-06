using System.Collections.Generic;
using RealDream.AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace RealDream.Replay
{
    [System.Serializable]
    public struct TransformData
    {
        public Vector3 Pos;
        public Vector3 Rot;
        public Vector3 Scale;
    }

    [System.Serializable]
    public struct AnimationInfo
    {
        public string Name;
        public float NormalizeTime;
        public float Weight;
    }

    [System.Serializable]
    public struct AnimData
    {
        public AnimationInfo Anim1;
        public AnimationInfo Anim2;
    }


    [System.Serializable]
    public struct AgentData
    {
        public int InstanceId;
        public int AssetId;
        public TransformData Transform;
        public AnimData Animation;
    }
    [System.Serializable]
    public class FrameData
    {
        public int CurId;
        public List<AgentData> Agents = new List<AgentData>();

        public override string ToString()
        {
            return $"CurId{CurId } count{Agents.Count}";
        }
    }
}