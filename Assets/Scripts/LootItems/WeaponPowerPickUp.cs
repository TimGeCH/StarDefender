using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPowerPickUp : LootItem
{
    [SerializeField] AudioData fullPowerPickUpSFX;
    

    protected override void PickUp()
    {
        if (player.IsFullPower)
        {
            pickUpSFX = fullPowerPickUpSFX;
            lootMessage.text = "DAMAGE UP!";
            player.IncreaseProjectileDamage();
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = "POWER UP!";
            player.PowerUp();
        }

        base.PickUp();
    }
}
