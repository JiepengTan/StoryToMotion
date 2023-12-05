using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace RealDream.AI
{
    [Category("Rdx")]
    public class MoveToTarget : BasicTask
    {
        public float keepDistance = 0.5f;

        protected override string info =>
            "MoveTo Target  " + (owner?.TargetActor == null ? "" : owner.TargetActor.ToString());

        protected NavMeshAgent navAgent;

        protected override void OnInit()
        {
            base.OnInit();
            navAgent = owner.GetComponent<NavMeshAgent>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (owner.TargetActor == null) return;
            navAgent.SetDestination(owner.TargetActor.transform.position);
            Debug.Log("Set SetDestination " + owner.TargetActor.transform.position);
        }

        private float timer = 0;

        protected override ETaskStatus OnUpdate(float dt)
        {
            if (owner.TargetActor == null)
            {
                return ETaskStatus.Failed;
            }

            timer += dt;
            if (timer > 1)
            {
                timer = 0;
                navAgent.SetDestination(owner.TargetActor.transform.position);
            }


            if (Vector3.Distance(navAgent.transform.position, owner.TargetActor.transform.position) <=
                navAgent.stoppingDistance + keepDistance)
            {
                return ETaskStatus.Success;
            }

            return ETaskStatus.Continue;
        }


        protected override void OnExit()
        {
            if (navAgent.gameObject.activeSelf)
            {
                navAgent.ResetPath();
            }
        }
    }
}