using System;

[Serializable]
public class ObstacleData : EntityData
{
    public LociPattern[] patterns;
    public bool loopPatterns = true;
}
