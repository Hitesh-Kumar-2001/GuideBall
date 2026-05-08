using UnityEngine;

public class LoaderForDevTesting : MonoBehaviour
{
    [Header("Ball")]
    public string ballName     = "ballSample";
    public Vector3 ballSpawn   = new Vector3(-2f, 0f, 0f);

    [Header("Obstacle")]
    public string obstacleName   = "obstacleSample";
    public Vector3 obstacleSpawn = new Vector3(2f, 0f, 0f);

    void Start()
    {
        BallPrefabFetcher.Instance.Spawn(ballName, ballSpawn);
        ObstaclePrefabFetcher.Instance.Spawn(obstacleName, obstacleSpawn);
    }
}
