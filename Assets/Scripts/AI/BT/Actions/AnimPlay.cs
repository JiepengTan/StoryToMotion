using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEditor.Rendering;
using UnityEngine;

namespace RealDream.AI
{
    [Category("Rdx/Anim")]
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