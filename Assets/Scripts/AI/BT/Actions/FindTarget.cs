using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace RealDream.AI
{
    [Category("Rdx/Search")]
    [Description("Action will end in Failure if no objects are found")]
    public class FindClosest : ActionTask<AIAgent>
    {
        [RequiredField]
        public BBParameter<string> tag;
        [BlackboardOnly]
        public BBParameter<Actor> saveAs;

        protected override string info {
            get { return "Find Closest " + tag + " result = " +(saveAs.value == null ?"":saveAs.value.Id.ToString()) ; }
        }

        protected override void OnExecute() {
            var actors = WorldContext.Instance.GetActors(tag.value);
            var pos = agent.transform.position;
            saveAs.value = actors.OrderBy(x => Vector3.Distance(x.transform.position, pos)).FirstOrDefault();
            EndAction(saveAs.value != null);
        }
    }
}