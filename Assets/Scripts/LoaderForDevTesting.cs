using UnityEngine;

public class LoaderForDevTesting : MonoBehaviour
{
    public string ballName = "ballSample";
    public Vector3 spawnPosition = Vector3.zero;

    void Start()
    {
        BallPrefabFetcher.Instance.Spawn(ballName, spawnPosition);
    }
}
