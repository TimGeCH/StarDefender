using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBonusPickUp : LootItem
{
    [SerializeField] int scoreBonus;

    protected override void PickUp()
    {
        ScoreManager.Instance.AddScore(scoreBonus);
        base.PickUp();
    }
}
