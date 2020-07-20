using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutAndDestroyComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeOutStep = 0.01f;
    private float startAlpha;

    private void Awake()
    {
        startAlpha = spriteRenderer.color.a;
    }

    //private void Start()
    //{
    //    //
    //}

    private void OnEnable()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, startAlpha);
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        float alpha = startAlpha;
        while (alpha > 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            alpha -= fadeOutStep;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }


}
