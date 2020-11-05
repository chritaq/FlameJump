using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashFx : MonoBehaviour
{
    [SerializeField] private SpriteRenderer overlayImage;

    private float colorAlpha = 0;

    public void StartScreenFlash(float time, float amount)
    {
        StartCoroutine(ScreenFlash(time, amount));
    }

    private IEnumerator ScreenFlash(float time, float amount)
    {
        //Flash on
        colorAlpha = amount;
        overlayImage.color = new Color(255, 255, 255, colorAlpha);

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Flash off
        colorAlpha = 0;
        overlayImage.color = new Color(255, 255, 255, colorAlpha);

        yield return null;
    }
}
