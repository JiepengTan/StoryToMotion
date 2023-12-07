using GamesTan;

namespace RealDream {
    [System.Serializable]
    public abstract partial class BaseLifecycle {
        public virtual void DoAwake() { }
        public virtual void DoStart() { }
        public virtual void DoFrameReset() { }
        public virtual void DoUpdate(float dt) { }
        public virtual void DoFixedUpdate(float dt) { }
        public virtual void DoDestroy() { }
    }
}