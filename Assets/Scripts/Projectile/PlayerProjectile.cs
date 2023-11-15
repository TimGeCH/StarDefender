using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    public static float baseDamage = 1.0f;

    TrailRenderer trail;

    protected virtual void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();

        if (moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }

    protected override void OnEnable()
    {
        damage = baseDamage;
        StartCoroutine(MoveDirctly());
    }

    void Ondisable()
    {
        trail.Clear();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);
    }

    public void IncreaseBaseDamage(float increase)
    {
        baseDamage += increase;
    }

    public static void ResetBaseDamage()
    {
        baseDamage = 1.0f;
    }
}
