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

    public void StartTransition(int fadeTime, bool fadeIn)
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

    private IEnumerator Transition(int fadeTime, bool fadeIn)
    {
        int timer = fadeTime;

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
        

        while (timer > 0)
        {
            
            //FadeColor += amount
            if (fadeIn)
            {
                colorAlpha += (float)1 / fadeTime;
            }
            else
            {
                colorAlpha -= (float)1 / fadeTime;
            }

            //overlayColor = new Color(255, 255, 255, colorAlpha);
            overlayImage.color = new Color(255, 255, 255, colorAlpha); 
            timer--;
            yield return new WaitForEndOfFrame();
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
