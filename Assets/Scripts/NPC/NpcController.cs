using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

namespace RealDream
{
    //this assure it's runned before any behaviour that may use it, as the animator need to be fecthed
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcController : MonoBehaviour
    {
        public bool interpolateTurning = false;

        public Animator animator { get { return m_Animator; } }
        public bool grounded { get { return m_Grounded; } }

        protected NavMeshAgent m_NavMeshAgent;
        protected bool m_FollowNavmeshAgent;
        protected Animator m_Animator;
        protected bool m_UnderExternalForce;
        protected bool m_ExternalForceAddGravity = true;
        protected Vector3 m_ExternalForce;
        protected bool m_Grounded;

        protected Rigidbody m_Rigidbody;

        const float k_GroundedRayDistance = .8f;

        void OnEnable()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            if (m_Rigidbody == null)
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            m_FollowNavmeshAgent = true;
        }

        private void FixedUpdate()
        {
            animator.speed = PlayerInput.Instance != null && PlayerInput.Instance.HaveControl() ? 1.0f : 0.0f;

            CheckGrounded();

            if (m_UnderExternalForce)
                ForceMovement();
        }

        void CheckGrounded()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            m_Grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
                QueryTriggerInteraction.Ignore);
        }
        public void MeleeAttackStart(int throwing = 0)
        {
        }

        // This is called by an animation event when Ellen finishes swinging her staff.
        public void MeleeAttackEnd()
        {
        }
        void ForceMovement()
        {
            if(m_ExternalForceAddGravity)
                m_ExternalForce += Physics.gravity * Time.deltaTime;

            RaycastHit hit;
            Vector3 movement = m_ExternalForce * Time.deltaTime;
            if (!m_Rigidbody.SweepTest(movement.normalized, out hit, movement.sqrMagnitude))
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            }

            m_NavMeshAgent.Warp(m_Rigidbody.position);
        }

       

        // used to disable position being set by the navmesh agent, for case where we want the animation to move the enemy instead (e.g. Chomper attack)
        public void SetFollowNavmeshAgent(bool follow)
        {
            if (!follow && m_NavMeshAgent.enabled)
            {
                m_NavMeshAgent.ResetPath();
            }
            else if(follow && !m_NavMeshAgent.enabled)
            {
                m_NavMeshAgent.Warp(transform.position);
            }

            m_FollowNavmeshAgent = follow;
            m_NavMeshAgent.enabled = follow;
        }

        public void AddForce(Vector3 force, bool useGravity = true)
        {
            if (m_NavMeshAgent.enabled)
                m_NavMeshAgent.ResetPath();

            m_ExternalForce = force;
            m_NavMeshAgent.enabled = false;
            m_UnderExternalForce = true;
            m_ExternalForceAddGravity = useGravity;
        }

        public void ClearForce()
        {
            m_UnderExternalForce = false;
            m_NavMeshAgent.enabled = true;
        }

        public void SetForward(Vector3 forward)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);

            if (interpolateTurning)
            {
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                    m_NavMeshAgent.angularSpeed * Time.deltaTime);
            }

            transform.rotation = targetRotation;
        }

        public bool SetTarget(Vector3 position)
        {
            return m_NavMeshAgent.SetDestination(position);
        }
    }
}