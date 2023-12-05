using UnityEngine;

namespace RealDream.AI
{
    public class AIAgent : MonoBehaviour
    {
        public AgentContext Agent;
    }

    public class AgentContext
    {
        public Actor TargetActor;
        public Vector3 TargetPos;
    }
}