using System.Collections.Generic;
using System.Text;

namespace RealDream.AI
{
    
    public class SequenceAction : BasicTask
    {
        private List<BasicTask> tasks = new List<BasicTask>();
        private int curTaskIdx;

        protected override string info
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (curTaskIdx < 0 || curTaskIdx >= tasks.Count) return "";
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (i == curTaskIdx)
                        sb.Append("-> ");
                    sb.AppendLine(tasks[i].ToString());
                }

                return sb.ToString();
            }
        }

        public SequenceAction Add(BasicTask task)
        {
            tasks.Add(task);
            return this;
        }

        public override void Init(AIAgent owner)
        {
            this.owner = owner;
            foreach (var task in tasks)
            {
                task.Init(owner);
            }
            OnInit();
        }

        protected override void OnStart()
        {
            base.OnStart();
            curTaskIdx = 0;
            foreach (var task in tasks)
            {
                task.Start();
            }
        }

        protected override ETaskStatus OnUpdate(float dt)
        {
            for (int i = curTaskIdx; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (!task.hasInit)
                {
                    task.Start();
                }

                var result = task.Update(dt);
                if (result == ETaskStatus.Continue)
                {
                    return result;
                }
                if (result == ETaskStatus.Failed)
                {
                    task.Exit();
                    return result;
                }
                curTaskIdx++;
                task.Exit();
            }
            return ETaskStatus.Success;
        }
    }
}