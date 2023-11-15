using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpwner : MonoBehaviour
{
    [SerializeField] LootSetting[] lootSettings;

    public void Spawn(Vector2 position)
    {
        foreach (var item in lootSettings)
        {
            item.Spawn(position + Random.insideUnitCircle);
        }
    }
}
