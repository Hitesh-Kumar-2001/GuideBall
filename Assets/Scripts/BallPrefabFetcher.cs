using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallPrefabFetcher : MonoBehaviour
{
    static BallPrefabFetcher instance;
    public static BallPrefabFetcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<BallPrefabFetcher>();
                if (instance == null)
                {
                    instance = new GameObject("BallPrefabFetcher").AddComponent<BallPrefabFetcher>();
                    Debug.Log("[BallPrefabFetcher] Auto-created singleton.");
                }
            }
            return instance;
        }
    }

    // Assets/Balls/Prefab/<name>.prefab  — must have BallController attached
    // Assets/Balls/BallData/<name>.json
    const string PrefabSubfolder = "Balls/Prefab/";
    const string DataSubfolder   = "Balls/BallData/";

    readonly Dictionary<string, BallData>   dataCache   = new();
    readonly Dictionary<string, GameObject> prefabCache = new();

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[BallPrefabFetcher] Singleton ready.");
    }

    public GameObject GetPrefab(string ballName)
    {
        if (prefabCache.TryGetValue(ballName, out var cached))
            return cached;

#if UNITY_EDITOR
        var path = $"Assets/{PrefabSubfolder}{ballName}.prefab";
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { Debug.LogError($"[BallPrefabFetcher] Prefab NOT found at: {path}"); return null; }
        var contents = UnityEditor.PrefabUtility.LoadPrefabContents(path);
        contents.AddComponent<BallController>();
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(contents, path);
        UnityEditor.PrefabUtility.UnloadPrefabContents(contents);
        prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Debug.Log($"[BallPrefabFetcher] BallController auto-added to '{ballName}' prefab.");
        prefabCache[ballName] = prefab;
        return prefab;
#else
        Debug.LogError("[BallPrefabFetcher] Prefab loading only works in the Editor. Move prefabs to Resources for builds.");
        return null;
#endif
    }

    public BallData GetData(string ballName)
    {
        if (dataCache.TryGetValue(ballName, out var cached))
            return cached;

        var path = Path.Combine(Application.dataPath, DataSubfolder.Replace('/', Path.DirectorySeparatorChar), ballName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogError($"[BallPrefabFetcher] JSON NOT found at: {path}");
            return null;
        }

        var data = JsonUtility.FromJson<BallData>(File.ReadAllText(path));
        dataCache[ballName] = data;
        Debug.Log($"[BallPrefabFetcher] Data '{ballName}' loaded — speed:{data.moveSpeed} rot:{data.rotationSpeed}");
        return data;
    }

    public BallController Spawn(string ballName, Vector3 position, Quaternion rotation = default)
    {
        var prefab = GetPrefab(ballName);
        var data   = GetData(ballName);

        if (prefab == null) { Debug.LogError("[BallPrefabFetcher] Spawn aborted — prefab is null."); return null; }
        if (data == null)   { Debug.LogError("[BallPrefabFetcher] Spawn aborted — data is null.");   return null; }

        var go         = Instantiate(prefab, position, rotation == default ? Quaternion.identity : rotation);
        var controller = go.GetComponent<BallController>();
        controller.Initialize(data);
        return controller;
    }

    public void PreloadAll()
    {
        var dataDir = Path.Combine(Application.dataPath, DataSubfolder.Replace('/', Path.DirectorySeparatorChar));
        if (!Directory.Exists(dataDir)) { Debug.LogError($"[BallPrefabFetcher] BallData folder not found: {dataDir}"); return; }

        foreach (var file in Directory.GetFiles(dataDir, "*.json"))
        {
            var ballName = Path.GetFileNameWithoutExtension(file);
            dataCache[ballName] = JsonUtility.FromJson<BallData>(File.ReadAllText(file));
            GetPrefab(ballName);
        }

        Debug.Log($"[BallPrefabFetcher] Preloaded {prefabCache.Count} ball(s).");
    }
}
