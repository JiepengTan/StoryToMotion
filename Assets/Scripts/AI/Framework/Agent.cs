using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RealDream.AI
{
    public class AIAgent : Actor
    {
        public Actor TargetActor;
        private BasicTask curTask;
        public Animator anim { get; private set; }
        public Queue<BasicTask> Tasks = new Queue<BasicTask>();

        protected override void OnAwake()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            DebugTask();
        }


        public void Update()
        {
            while (curTask != null)
            {
                var dt = Time.deltaTime;
                if (!curTask.hasInit)
                {
                    curTask.Start();
                }

                var result = curTask.Update(dt);
                if (result == ETaskStatus.Continue)
                {
                    return;
                }
                curTask.Exit();
                curTask = GetNextTask();
                Debug.Log("Task Done " + result);
            }
        }

        BasicTask GetNextTask()
        {
            if (Tasks.Count == 0)
                return null;
            return Tasks.Dequeue();
        }

        public void AddTask(BasicTask task)
        {
            task.Init(this);
            if (curTask == null)
            {
                curTask = task;
                return;
            }
            Tasks.Enqueue(task);
        }


        [Header("Debug")] //
        public string DebugTag = "";

        [ReadOnly]
        [MultiLineProperty(Lines = 10)]
        [ShowInInspector]
        public string DebugInfo => $"Count:{Tasks.Count} curTask:\n" + curTask?.ToString() ?? "";

        [Button]
        void DebugTask()
        {
            AddTask("Shop", "MeleeAttack");
            AddTask("NPC", "MeleeAttack");
            AddTask("GasStation", "MeleeAttack");
        }

        void AddTask(string tag, string endTriggerName)
        {
            AddTask(new SequenceAction()
                .Add(new FindClosest() { tag = tag })
                .Add(new MoveToTarget() { })
                .Add(new AnimSetTrigger() { triggerName = endTriggerName })
            );
        }
    }
}