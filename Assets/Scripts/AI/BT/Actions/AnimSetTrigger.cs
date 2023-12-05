using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEditor.Rendering;
using UnityEngine;

namespace RealDream.AI
{
    public enum ETaskStatus
    {
        Continue,
        Success,
        Failed
    }

    public class ActionExecute : MonoBehaviour
    {
    }

    [Category("Rdx/Anim")]
    public class AnimSetTrigger : BasicTask
    {
        public string triggerName = "";
        protected override string info => $"AnimSetTrigger {(triggerName )}";

        protected override ETaskStatus OnUpdate(float dt)
        {
            if (!string.IsNullOrEmpty(triggerName))
            {
                owner.anim.SetTrigger(triggerName);
            }

            return ETaskStatus.Success;
        }
    }
}