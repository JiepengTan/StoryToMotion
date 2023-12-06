using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace RealDream.AI
{
    [Category("Rdx/Util")]
    public class Wait : BasicTask
    {
        public float waitTime;
        protected override string info => $"Wait {waitTime} sec.";
        protected override ETaskStatus OnUpdate(float dt)
        {
            if (elapsedTime >= waitTime)
            {
                owner.anim.Play(PlayableAnimator.AnimName_Idle);
                return ETaskStatus.Success;
            }
            
            return ETaskStatus.Continue;
        }
    }
}