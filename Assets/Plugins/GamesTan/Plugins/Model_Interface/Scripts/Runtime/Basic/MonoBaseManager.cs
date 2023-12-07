using System.Text;
using GamesTan;
using UnityEngine;

namespace RealDream {
    public partial class MonoBaseManager : MonoBehaviour, IManager {
        public virtual void DoAwake() {
        }

        public virtual void DoStart() {
        }

        public virtual void DoUpdate(float deltaTime) {
        }

        public virtual void DoFixedUpdate(float deltaTime) {
        }

        public virtual void DoDestroy() {
        }
    }

    [System.Serializable]
    public class MonoBaseManager<T> : MonoBaseManager, IService where T : MonoBaseManager<T> {
        private static T _Instance;

        public static T Instance {
            get {
#if UNITY_EDITOR
                if (_Instance == null) {
                    _Instance = GameObject.FindObjectOfType<T>();
                    if (_Instance == null) {
                        var go = new GameObject(typeof(T).Name);
                        _Instance = go.AddComponent<T>();
                    }
                }
#endif
                return _Instance;
            }
        }

        private void Awake() {
            BindInstance();
        }

        public void BindInstance() {
            _Instance = (T) (object) this;
        }
    }
}