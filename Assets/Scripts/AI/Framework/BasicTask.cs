using System;
using NodeCanvas.Framework;
using UnityEngine.UIElements;

namespace RealDream.AI
{
    public abstract class BasicTask
    {
        protected AIAgent owner;

        public bool hasInit { get; private set; }

        public virtual void Init(AIAgent owner)
        {
            this.owner = owner;
            OnInit();
        }

        public void Start()
        {
            hasInit = true;
            elapsedTime = 0;
            OnStart();
        }
        
        protected float elapsedTime;

        public ETaskStatus Update(float dt)
        {
            elapsedTime += dt;
            return OnUpdate(dt);
        }

        public void Exit()
        {
            hasInit = false;
            OnExit();
        }
        
        
        protected virtual string info =>"";
        
        public override string ToString()
        {
            return info??"";
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual ETaskStatus OnUpdate(float dt)
        {
            return ETaskStatus.Success;
        }

        protected virtual void OnExit()
        {
        }
    }
}