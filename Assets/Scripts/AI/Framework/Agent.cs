using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealDream.AI
{
    public class AIAgent : Actor
    {
        public List<string> TaskCmds = new List<string>();
        public Actor TargetActor;
        private BasicTask curTask;
        public PlayableAnimator anim { get; private set; }
        public Queue<BasicTask> Tasks = new Queue<BasicTask>();

        protected override void OnAwake()
        {
            anim = GetComponentInChildren<PlayableAnimator>();
        }

            
        
        void DebugTask()
        {
            foreach (var line in TaskCmds)
            {
                var strs = line.Split(';');
                Debug.Log("Add Task: " + line);
                AddTask(strs[0].Trim(),strs[1].Trim());
            }
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
                if (curTask == null)
                {
                    ReplayManager.Instance?.OnTaskDone(this);
                }
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

        //[ReadOnly]
        //[MultiLineProperty(Lines = 10)]
        //[ShowInInspector]
        public string DebugInfo => $"Count:{Tasks.Count} curTask:\n" + curTask?.ToString() ?? "";


        void AddTask(string tag, string endTriggerName)
        {
            AddTask(new SequenceAction()
                .Add(new FindClosest() { tag = tag })
                .Add(new MoveToTarget() { })
                .Add(new AnimPlay() { triggerName = endTriggerName })
                .Add(new Wait() {waitTime = 2})
            );
        }
    }
}