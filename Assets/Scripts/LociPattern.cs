using System;
using UnityEngine;

// 0=Circle  1=UpDown  2=LeftRight  3=Figure8
public enum LociType { Circle, UpDown, LeftRight, Figure8 }

[Serializable]
public class LociPattern
{
    public LociType type      = LociType.Circle;
    public float radius       = 2f;   // circle radius
    public float amplitude    = 2f;   // oscillation height/width
    public float frequency    = 0.5f; // cycles per second
    public float duration     = 0f;   // seconds on this pattern; 0 = forever
}
