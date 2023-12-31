using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RealDream.AI
{
    public class FindClosest : BasicTask
    {
        public string tag ;

        protected override string info =>
            $"Find Closest {tag} result =  {(owner?.TargetActor == null ? "" : owner.TargetActor.InstanceId.ToString())}";


        protected override ETaskStatus OnUpdate(float dt)
        {
            var actors = WorldContext.Instance.GetActors(tag);
            var pos = owner.transform.position;
            owner.TargetActor = actors.OrderBy(x => Vector3.Distance(x.transform.position, pos)).FirstOrDefault();
            if (owner.TargetActor != null)
            {
                //Debug.LogError("Pos " +  owner.TargetActor.transform.position);
            }
            else
            {
                Debug.LogError("Find error " + tag);
            }

            return owner.TargetActor == null ? ETaskStatus.Failed : ETaskStatus.Success;
        }
    }
}