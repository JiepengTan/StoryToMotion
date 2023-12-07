namespace RealDream {
    [System.Serializable]
    public abstract class BaseManager : BaseLifecycle , IManager{
        public static bool HasStart { get; protected set; }
        
        public virtual void OnLoadLevel() { }

        public virtual void ClearUp() { }

        public virtual void DrawGizmos() { }
       
    }

    [System.Serializable]
    public abstract class BaseManager<T> : BaseManager where T : class, new() {
        private static T _Instance;

        public static T Instance {
            get {
                if (_Instance == null) {
                    _Instance = new T();
                }

                return _Instance;
            }
        }

        public override void DoAwake() {
            HasStart = true;
            _Instance = ((object) this) as T;
        }
    }
}