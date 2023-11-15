using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;
    [SerializeField] float fillSpeed = 0.1f;
    

    float currentFillAmount;
    protected float targetFillAmount;
    float previousFillAmount;
    float t;

    WaitForSeconds waitForDelayFill;

    Coroutine bufferedFillingCorouting;

    Canvas canvas;

    void Awake()
    {
        if(TryGetComponent<Canvas> (out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    void Ondisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCorouting != null)
        {
            StopCoroutine(bufferedFillingCorouting);
        }

        if (currentFillAmount > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferedFillingCorouting = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
        }

        else if (currentFillAmount < targetFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;
            bufferedFillingCorouting = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
        
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {

        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        previousFillAmount = currentFillAmount;
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
        
    }
}
