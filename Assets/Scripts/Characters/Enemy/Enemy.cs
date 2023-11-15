using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int scorePoint = 100;
    [SerializeField] int deathEnergyBouns = 3;
    [SerializeField] protected int healthFactor;

    LootSpwner lootSpawer;

    protected virtual void Awake()
    {
        lootSpawer = GetComponent<LootSpwner>();
    }

    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }


    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }


    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBouns);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawer.Spawn(transform.position);
        base.Die();
    }

    protected virtual void SetHealth()
    {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber / healthFactor);
    }
}
