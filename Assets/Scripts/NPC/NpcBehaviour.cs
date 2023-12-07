using UnityEngine;
using UnityEngine.AI;

namespace RealDream
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcBehaviour : ActorComponent
    {
     
        protected PlayableAnimator animator;
        protected NavMeshAgent _navMeshAgent;


        [Range(0.01f,10)]
        public float AnimSpeed = 3;
        private Vector3 _lastPos;

        [Header("Debug")] //
        public Transform target;

        private float _timer;
        public float forwardSpeed;


        void OnEnable()
        {
            animator = GetComponentInChildren<PlayableAnimator>();
            _navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            _lastPos = transform.position;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (target != null)
            {
                _timer += Time.deltaTime;
                if (_timer > 1)
                {
                    _timer = 0;
                    SetTarget(target.position);
                }
            }

            var delta = (transform.position - _lastPos);
            var vel = delta / Time.deltaTime;
            vel.y = 0;
            var targetSpeed = vel.magnitude * AnimSpeed;
            forwardSpeed = Mathf.Lerp(forwardSpeed, targetSpeed, 0.1f);
            animator.Move(new Vector2(0,forwardSpeed));
            _lastPos = transform.position;
        }
        
        public bool SetTarget(Vector3 position)
        {
            return _navMeshAgent.SetDestination(position);
        }

    }
}