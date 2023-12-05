using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RealDream.AI
{
    public class AIAgent : Actor
    {
        public Actor TargetActor;
        private SequenceAction curTask;
        public Animator anim { get; private set; }

        protected override void OnAwake()
        {
            anim = GetComponentInChildren<Animator>();
        }


        public void Update()
        {
            if (curTask == null)
            {
                return;
            }

            var dt = Time.deltaTime;
            if (!curTask.hasInit)
            {
                curTask.Start();
            }

            var result = curTask.Update(dt);
            if (result != ETaskStatus.Continue)
            {
                curTask.Exit();
                curTask = null;
                Debug.Log("Task Done " + result);
                return;
            }

        }

        
        public void RunTask(SequenceAction task)
        {
            if (curTask != null)
            {
                curTask.Exit();
            }
            curTask = task;
            task.Init(this);
            Debug.Log($"{Id} RunTask \n" + task);
        }


        [Header("Debug")] //
        public string DebugTag = "Shop";

        public string DebugAnimTrigger = "MeleeAttack";
        [ReadOnly][MultiLineProperty][ShowInInspector]
        public string DebugInfo=>  curTask?.ToString()??"";

        [Button]
        void DebugTask()
        {
            AddTask(DebugTag, DebugAnimTrigger);
        }

        void AddTask(string tag,string endTriggerName)
        {
            RunTask(new SequenceAction()
                .Add(new FindClosest() { tag = tag })
                .Add(new MoveToTarget() { })
                .Add(new AnimSetTrigger() { triggerName = endTriggerName })
            );
        }
    }
}