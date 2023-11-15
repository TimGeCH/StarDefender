using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar_HUD : StatsBar
{
    [SerializeField] protected Text prencentText;


    protected virtual void SetPrecentText()
    {
        //prencentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
        prencentText.text = targetFillAmount.ToString("P0");
    }

    public override void Initialize(float currentValue, float maxValue)
    {
        base.Initialize(currentValue, maxValue);
        SetPrecentText();
    }

    protected override IEnumerator BufferedFillingCoroutine(Image image)
    {
        SetPrecentText();
        return base.BufferedFillingCoroutine(image);
    }
}
