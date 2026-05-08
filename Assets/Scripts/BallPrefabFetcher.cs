using UnityEngine;

public class BallPrefabFetcher : EntityPrefabFetcher<BallData, BallController>
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
                    instance = new GameObject("BallPrefabFetcher").AddComponent<BallPrefabFetcher>();
            }
            return instance;
        }
    }

    // Assets/Balls/Prefab/<name>.prefab
    // Assets/Balls/BallData/<name>.json
    protected override string PrefabSubfolder => "Balls/Prefab/";
    protected override string DataSubfolder   => "Balls/BallData/";

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
