using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class EntityPrefabFetcher<TData, TController> : MonoBehaviour
    where TData    : EntityData
    where TController : EntityController
{
    protected abstract string PrefabSubfolder { get; }
    protected abstract string DataSubfolder   { get; }

    readonly Dictionary<string, TData>      dataCache   = new();
    readonly Dictionary<string, GameObject> prefabCache = new();

    public GameObject GetPrefab(string entityName)
    {
        if (prefabCache.TryGetValue(entityName, out var cached))
            return cached;

#if UNITY_EDITOR
        var path   = $"Assets/{PrefabSubfolder}{entityName}.prefab";
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { Debug.LogError($"[{GetType().Name}] Prefab NOT found at: {path}"); return null; }
        prefabCache[entityName] = prefab;
        return prefab;
#else
        var prefab = Resources.Load<GameObject>($"{PrefabSubfolder}{entityName}");
        if (prefab == null)
            Debug.LogError($"[{GetType().Name}] Prefab not found in Resources: {PrefabSubfolder}{entityName}");
        else
            prefabCache[entityName] = prefab;
        return prefab;
#endif
    }

    public TData GetData(string entityName)
    {
        if (dataCache.TryGetValue(entityName, out var cached))
            return cached;

        var path = Path.Combine(Application.dataPath, DataSubfolder.Replace('/', Path.DirectorySeparatorChar), entityName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogError($"[{GetType().Name}] JSON NOT found at: {path}");
            return null;
        }

        var data = JsonUtility.FromJson<TData>(File.ReadAllText(path));
        dataCache[entityName] = data;
        return data;
    }

    public TController Spawn(string entityName, Vector3 position, Quaternion rotation = default)
    {
        var prefab = GetPrefab(entityName);
        var data   = GetData(entityName);

        if (prefab == null) { Debug.LogError($"[{GetType().Name}] Spawn aborted — prefab is null."); return null; }
        if (data == null)   { Debug.LogError($"[{GetType().Name}] Spawn aborted — data is null.");   return null; }

        var go         = Instantiate(prefab, position, rotation == default ? Quaternion.identity : rotation);
        var controller = go.GetComponent<TController>() ?? go.AddComponent<TController>();
        controller.Initialize(data);
        return controller;
    }

    public void PreloadAll()
    {
        var dataDir = Path.Combine(Application.dataPath, DataSubfolder.Replace('/', Path.DirectorySeparatorChar));
        if (!Directory.Exists(dataDir)) { Debug.LogError($"[{GetType().Name}] Data folder not found: {dataDir}"); return; }

        foreach (var file in Directory.GetFiles(dataDir, "*.json"))
        {
            var entityName = Path.GetFileNameWithoutExtension(file);
            dataCache[entityName] = JsonUtility.FromJson<TData>(File.ReadAllText(file));
            GetPrefab(entityName);
        }

        Debug.Log($"[{GetType().Name}] Preloaded {prefabCache.Count} entity/entities.");
    }
}
