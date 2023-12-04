using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealDream
{
    public interface IAction
    {
        void OnStart();
        void OnEnd();
        void OnUpdate();
    }

    public class BaseAction : IAction
    {
        public virtual void OnStart()
        {
            
        }

        public virtual void OnEnd()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}