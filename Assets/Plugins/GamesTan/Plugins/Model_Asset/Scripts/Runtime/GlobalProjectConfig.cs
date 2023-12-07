using UnityEngine;

namespace RealDream {
    // 全局配置 必须放在 Resources 目录下
    public partial class GlobalProjectConfig : BasesGlobalConfig{ 
        protected static GlobalProjectConfig _Instance;
        public static GlobalProjectConfig Instance {
            get {
                if (_Instance == null) {
                    var config = Resources.Load<GlobalProjectConfig>( "GlobalProjectConfig");
                    _Instance = config;
                    config.DoInit();
                }

                return _Instance;
            }
        }
        [Header("Directory")]
        [Tooltip("资源根目录")]
        public string RawResDir = "Assets/GamesTan/UIFramework/Res/";
        [Tooltip("资源根目录(可代码直接加载,AB打包路径)")]
        public string ResourcesDir = "Assets/GamesTan/UIFramework/Res/Resources/";
        
        [Header("UI")]
        [Tooltip("UI 预制体相对路径")]
        public string UIPrefabRelDir = "Prefabs/UI/";
        [Tooltip("UI 命名空间")]
        public string UINamespace = "GamesTan.UI.";

        public override void DoInit() {
            ResourcesDir = CheckDir(ResourcesDir);
            RawResDir = CheckDir(RawResDir);
            UINamespace = CheckNameSpace(UINamespace);
            UIPrefabRelDir = CheckDir(UIPrefabRelDir);
        }
        string CheckNameSpace(string dir) {
            if (!dir.EndsWith(".")) {
                dir = dir + ".";
            }
            return dir;
        }
        string CheckDir(string dir) {
            dir = dir.Replace("\\", "/");
            if (!dir.EndsWith("/")) {
                dir = dir + "/";
            }
            return dir;
        }

    }
}