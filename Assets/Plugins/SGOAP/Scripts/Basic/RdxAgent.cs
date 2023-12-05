using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SGoap
{
    public class RdxAgent : MonoBehaviour
    {
        public List<Goal> AllGoals;
        private Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
            Update();
        }

        public void SetGoal(string goalName)
        {
            if (agent == null) return;
            var goal = AllGoals.Find(a => a.Key == goalName);
            if (goal == null) return;
            agent.Goals.Clear();
            agent.Goals.Add(goal);
        }
        [Range(0,4)]

        public int DebugIdx;

        public void Update()
        {
            if (DebugIdx < 0 || DebugIdx >= AllGoals.Count) return;
            SetGoal(AllGoals[DebugIdx].Key);
        }
    }
}