using System.Collections.Generic;
using UnityEngine;


namespace RealDream {
    public abstract partial class AssetConfig : BaseGlobalConfig<AssetConfig> {
        [Tooltip("游戏运行时刷新? (可能会导致不同步)")] public bool IsRefreshOnRuntime = true;

        [System.Serializable]
        public class AssetInfo {
            public int id;
            public string path;

            public override string ToString() {
                return "" + id + " " + path;
            }
        }

        [System.Serializable]
        public class ConfigInfo {
            public int id;
            public ScriptableObject config;
        }


        [Header("AssetConfig")] public List<AssetInfo> assetInfos = new List<AssetInfo>();
        public List<ConfigInfo> cfgInfos = new List<ConfigInfo>();

        private Dictionary<int, string> _id2Path = new Dictionary<int, string>();
        private Dictionary<int, ScriptableObject> _id2Config = new Dictionary<int, ScriptableObject>();


        public override void DoInit() {
            _Instance = this;
            _id2Path.Clear();
            foreach (var info in assetInfos) {
                _id2Path[info.id] = info.path;
            }

            _id2Config.Clear();
            foreach (var info in cfgInfos) {
                _id2Config[info.id] = info.config;
            }
        }

        public string GetPath(int assetId) {
            string ret = null;
            if (_id2Path.TryGetValue(assetId, out ret)) return ret;
            return null;
        }

        public ScriptableObject GetConfig(int configId) {
            ScriptableObject ret = null;
            if (_id2Config.TryGetValue(configId, out ret)) return ret;
            return null;
        }
    }
}