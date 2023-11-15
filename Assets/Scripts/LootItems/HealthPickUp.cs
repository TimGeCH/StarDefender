using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : LootItem
{
    [SerializeField] AudioData fullHealthPickUpSFX;
    [SerializeField] int fullHealthScoreBonus = 200;
    [SerializeField] float HealthdBonus = 20f;

    protected override void PickUp()
    {
        if (player.IsFullHealth)
        {
            pickUpSFX = fullHealthPickUpSFX;
            lootMessage.text = $"SCORE + {fullHealthScoreBonus}";
            ScoreManager.Instance.AddScore(fullHealthScoreBonus);
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"HEALTH + {HealthdBonus}";
            player.RestoreHealth(HealthdBonus);
        }

        base.PickUp();
    }
}
