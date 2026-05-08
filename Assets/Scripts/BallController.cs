using UnityEngine;

public class BallController : EntityController
{
    public bool IsDead { get; private set; }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<ObstacleController>() != null)
            Die();
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log("[BallController] Ball died — hit an obstacle.");
        // TODO: play death animation, trigger game over, disable movement, etc.
    }
}
