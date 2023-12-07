using System;
using GamesTan;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RealDream {
    public class AssetsUtil {
        private static IAssetLoader _loader;

        public static void SetLoader(IAssetLoader loader) {
            _loader = loader;
        }

        public static T LoadUObject<T>(string path, string posfix = "") where T : Object =>
            _loader.LoadUObject<T>(path, posfix);

        public static T LoadAsset<T>(string path) where T : Object => _loader.LoadAsset<T>(path);

        public static string LoadText(string path, string posfix = ".json") => _loader.LoadText(path, posfix);

        public static GameObject LoadPrefab(string path) => _loader.LoadPrefab(path);
    }
}