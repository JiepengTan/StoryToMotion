using UnityEngine;

namespace RealDream {
    public interface IAssetLoader {
        T LoadUObject<T>(string path, string posfix = "") where T : Object;

        T LoadAsset<T>(string path) where T : Object;
        string LoadText(string path, string posfix = ".json");
        GameObject LoadPrefab(string path);
    }
}