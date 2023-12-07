using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealDream
{
    public class EditorPool
    {
        private static Dictionary<int, Stack<GameObject>> pools = new Dictionary<int, Stack<GameObject>>();

        public static void DestroyAgent(GameObject tran)
        {
            var assetRef = tran.GetComponent<AssetRef>();
            if (assetRef == null)
            {
                GameObject.DestroyImmediate(assetRef);
            }

            var assetId = assetRef.AssetId;
            if (!pools.TryGetValue(assetId, out var lst))
            {
                pools[assetId] = new Stack<GameObject>();
            }

            var lst2 = pools[assetId];
            lst2.Push(tran);
            tran.transform.SetParent(null, true);
            tran.SetActive(false);
        }

        public static GameObject CreateAgent(int assetId)
        {
            if (pools.TryGetValue(assetId, out var lst) && lst.Count > 0)
            {
                var item = lst.Pop();
                item.transform.localScale = Vector3.one;
                item.SetActive(true);
                return item;
            }

            var prefab = ResourceManager.Instance.LoadPrefab(assetId);
            if (prefab == null)
            {
                Debug.LogError("Can not load Prefab  " + assetId);
                return null;
            }

            GameObject go = null;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            else
#endif
            {
                go = GameObject.Instantiate(prefab);
            }
            go.SetActive(true);
            return go;
        }
    }
}