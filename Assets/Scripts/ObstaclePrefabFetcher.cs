using UnityEngine;

public class ObstaclePrefabFetcher : EntityPrefabFetcher<ObstacleData, ObstacleController>
{
    static ObstaclePrefabFetcher instance;
    public static ObstaclePrefabFetcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ObstaclePrefabFetcher>();
                if (instance == null)
                    instance = new GameObject("ObstaclePrefabFetcher").AddComponent<ObstaclePrefabFetcher>();
            }
            return instance;
        }
    }

    // Assets/Obstacles/Prefab/<name>.prefab
    // Assets/Obstacles/ObstacleData/<name>.json
    protected override string PrefabSubfolder => "Obstacles/Prefab/";
    protected override string DataSubfolder   => "Obstacles/ObstacleData/";

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
