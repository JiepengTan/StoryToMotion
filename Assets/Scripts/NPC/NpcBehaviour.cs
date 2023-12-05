using UnityEngine;
using UnityEngine.AI;

namespace RealDream
{
    [RequireComponent(typeof(NpcController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcBehaviour : MonoBehaviour
    {
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");

        readonly int m_HashGrounded = Animator.StringToHash("Grounded");

        // States
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");


        protected NpcController _controller;
        protected NavMeshAgent _navMeshAgent;


        [Range(0.01f,10)]
        public float AnimSpeed = 3;
        private Vector3 _lastPos;

        [Header("Debug")] //
        public Transform target;

        private float _timer;

        void OnEnable()
        {
            _controller = GetComponent<NpcController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();


            _controller.animator.Play(m_HashLocomotion, 0, Random.value);

            _lastPos = transform.position;
        }

        public float forwardSpeed;

        private void Update()
        {
            if (target != null)
            {
                _timer += Time.deltaTime;
                if (_timer > 1)
                {
                    _timer = 0;
                    _controller.SetTarget(target.position);
                }
            }

            var delta = (transform.position - _lastPos);
            var vel = delta / Time.deltaTime;
            vel.y = 0;
            var targetSpeed = vel.magnitude * AnimSpeed;
            forwardSpeed = Mathf.Lerp(forwardSpeed, targetSpeed, 0.1f);
            _controller.animator.SetBool(m_HashGrounded, _controller.grounded);
            _controller.animator.SetFloat(m_HashForwardSpeed, forwardSpeed);

            _lastPos = transform.position;
        }
    }
}