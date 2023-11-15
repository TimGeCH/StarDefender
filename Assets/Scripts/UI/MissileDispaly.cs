using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileDispaly : MonoBehaviour
{
    static Text amountText;
    static Image cooldownImage;

    void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>();
    }

    public static void UpdataAmountText (int amount)
    {
        amountText.text = amount.ToString();
    }

    public static void UpdataCooldownImage(float fillAmount)
    {
        cooldownImage.fillAmount = fillAmount;
    }


   
   
}
