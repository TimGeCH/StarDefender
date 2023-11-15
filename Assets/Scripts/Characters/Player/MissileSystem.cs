using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 5;
    [SerializeField] GameObject missilePrefab = null;
    [SerializeField] AudioData launchSFX = null;
    [SerializeField] float soundCooldownTime = 2f;
    [SerializeField] AudioData outOfMissilesSFX = null;

    float soundTimer = 0f;

    bool isReady = true;

    int amount;

    void Awake()
    {
        amount = defaultAmount; 
    }

    void Start()
    {
        MissileDispaly.UpdataAmountText(amount);
    }


    void Update()
    {
        if (soundTimer > 0) // 如果音效计时器大于0，就让它递减
        {
            soundTimer -= Time.deltaTime;
        }
    }

    public void PickUp()
    {
        amount++;
        MissileDispaly.UpdataAmountText(amount);

        if (amount == 1)
        {
            MissileDispaly.UpdataCooldownImage(0f);
            isReady = true;
        }
    }

    public void Launch(Transform muzzleTransform)
    {
        if (amount == 0)
        {
            if (soundTimer <= 0) // 只有当音效计时器小于等于0时，才播放音效
            {
                AudioManager.Instance.PlayRandomSFX(outOfMissilesSFX);
                soundTimer = soundCooldownTime; // 重置音效计时器
            }
            return;
        }
        else if (!isReady)
        {
            return;
        }

        isReady = false;
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(launchSFX);
       
        amount--;
        MissileDispaly.UpdataAmountText(amount);

        if (amount == 0)
        {
            MissileDispaly.UpdataCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f)
        {
            MissileDispaly.UpdataCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

       isReady = true;
       
    }
       
}