using UnityEngine;

public class ObstacleController : EntityController
{
    ObstacleData data;
    Vector2 anchor;
    int     patternIndex;
    float   patternTimer;
    float   patternTime;     // resets each time a new pattern starts

    public override void Initialize(EntityData entityData)
    {
        base.Initialize(entityData);
        data         = entityData as ObstacleData;
        anchor       = transform.position;
        patternIndex = 0;
        patternTimer = 0f;
        patternTime  = 0f;

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected override void Move()
    {
        if (data?.patterns == null || data.patterns.Length == 0) return;

        var pattern = data.patterns[patternIndex];

        patternTime  += Time.fixedDeltaTime;
        patternTimer += Time.fixedDeltaTime;

        if (pattern.duration > 0f && patternTimer >= pattern.duration)
            AdvancePattern();

        var rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(anchor + ComputeOffset(data.patterns[patternIndex], patternTime));
    }

    void AdvancePattern()
    {
        patternTimer = 0f;
        patternTime  = 0f;
        int next = patternIndex + 1;

        if (next >= data.patterns.Length)
        {
            if (data.loopPatterns) patternIndex = 0;
            // else stay on last pattern indefinitely
        }
        else
        {
            patternIndex = next;
        }
    }

    static Vector2 ComputeOffset(LociPattern p, float t)
    {
        float angle = t * p.frequency * Mathf.PI * 2f;

        return p.type switch
        {
            LociType.Circle    => new Vector2(Mathf.Cos(angle) * p.radius,
                                              Mathf.Sin(angle) * p.radius),

            LociType.UpDown    => new Vector2(0f,
                                              Mathf.Sin(angle) * p.amplitude),

            LociType.LeftRight => new Vector2(Mathf.Sin(angle) * p.amplitude,
                                              0f),

            LociType.Figure8   => new Vector2(Mathf.Sin(angle)       * p.amplitude,
                                              Mathf.Sin(angle * 2f)  * p.amplitude * 0.5f),
            _ => Vector2.zero
        };
    }
}
