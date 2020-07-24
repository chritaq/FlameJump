using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionFx : MonoBehaviour
{
    //[SerializeField] private Color overlayColor = new Color(0,0,0,0);

    [SerializeField] private RawImage overlayImage;
    private Coroutine transitionCoroutine;
    [SerializeField] private Animator swipeAnimator;

    private float colorAlpha = 0;

    public void StartTransition(float fadeTime, bool fadeIn)
    {
        if(transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(Transition(fadeTime, fadeIn));
    }

    public void StartSwipe(bool fadeIn)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        overlayImage.color = new Color(255, 255, 255, 0);
        swipeAnimator.SetBool("fadeIn", fadeIn);
    }

    private IEnumerator Transition(float fadeStep, bool fadeIn)
    {
        float step = fadeStep / 100;

        if(fadeIn)
        {
            //overlayColor = new Color(255, 255, 255, 0);
            overlayImage.color = new Color(255, 255, 255, 0);
            colorAlpha = 0;
        }
        else
        {
            //overlayColor = new Color(255, 255, 255, 255);
            overlayImage.color = new Color(255, 255, 255, 1);
            colorAlpha = 1;
        }


        while (true)
        {
            
            //FadeColor += amount
            if (fadeIn)
            {
                colorAlpha += step;
                if(colorAlpha >= 1)
                {
                    break;
                }
            }
            else
            {
                colorAlpha -= step;
                if (colorAlpha <= 1)
                {
                    break;
                }
            }

            overlayImage.color = new Color(255, 255, 255, colorAlpha); 
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if (fadeIn)
        {
            overlayImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            overlayImage.color = new Color(255, 255, 255, 0);
        }
    }
}
