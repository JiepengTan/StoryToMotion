

using UnityEngine;

namespace RealDream {
    public partial class BasesGlobalConfig  : ScriptableObject, IBaseGlobalConfig{ 
        public virtual void DoInit() { }
    }

    public partial class BaseGlobalConfig<T> :BasesGlobalConfig
        where T : ScriptableObject, IBaseGlobalConfig {
        protected static T _Instance;
        public static T Instance {
            get {
                if (_Instance == null) {
                    var config = AssetsUtil.LoadAsset<T>("Configs/Global/" + typeof(T).Name );
                    _Instance = config;
                    config.DoInit();
                }

                return _Instance;
            }
        }
    }
}