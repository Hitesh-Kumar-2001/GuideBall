using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : MonoBehaviour
{
    public Vector2 Direction { get; private set; }

    Rigidbody2D rb;
    float moveSpeed;
    float rotationSpeed;
    float boostMultiplier;
    float boostDuration;
    float currentSpeed;
    bool isBoosted;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        Direction = Vector2.right;

        var col = GetComponent<Collider2D>();
        if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    public virtual void Initialize(EntityData data)
    {
        moveSpeed       = data.moveSpeed;
        rotationSpeed   = data.rotationSpeed;
        boostMultiplier = data.boostMultiplier;
        boostDuration   = data.boostDuration;
        currentSpeed    = moveSpeed;
    }

    void FixedUpdate() => Move();

    protected virtual void Move()
    {
        rb.linearVelocity = Direction * currentSpeed;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void ChangeDirection(Vector2 newDirection) => Direction = newDirection.normalized;
    public void FlipDirectionX() => Direction = new Vector2(-Direction.x,  Direction.y);
    public void FlipDirectionY() => Direction = new Vector2( Direction.x, -Direction.y);

    public void RandomizeDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void Accelerate()
    {
        if (!isBoosted)
            StartCoroutine(BoostCoroutine());
    }

    IEnumerator BoostCoroutine()
    {
        isBoosted    = true;
        currentSpeed = moveSpeed * boostMultiplier;
        yield return new WaitForSeconds(boostDuration);
        currentSpeed = moveSpeed;
        isBoosted    = false;
    }
}
