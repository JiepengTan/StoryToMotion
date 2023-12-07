
using UnityEditor.Rendering;
using UnityEngine;

namespace RealDream.AI
{
    public class AnimPlay : BasicTask
    {
        public string triggerName = "";
        protected override string info => $"AnimPlay {(triggerName )}";

        protected override ETaskStatus OnUpdate(float dt)
        {
            if (!string.IsNullOrEmpty(triggerName))
            {
                owner.anim.Play(triggerName);
            }

            return ETaskStatus.Success;
        }
    }
}