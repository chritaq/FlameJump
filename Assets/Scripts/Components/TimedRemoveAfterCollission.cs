using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedRemoveAfterCollission : MonoBehaviour
{
    [SerializeField] private float timeBeforeDissapearing = 1f;
    [SerializeField] private bool respawnAfterTime = true;
    [SerializeField] private float respawnTime = 3f;


    private Collider2D collider;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particleSystem;

    private bool removing = false;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private Coroutine removingCoroutine;
    public void StartRemoveAfterCollision()
    {
        if(!removing)
        {
            removing = true;
            StopActiveCoroutine();
            removingCoroutine = StartCoroutine(TimedRemove());
        }
    }

    private void StopActiveCoroutine()
    {
        if (removingCoroutine != null)
        {
            StopCoroutine(removingCoroutine);
        }
    }

    private IEnumerator TimedRemove()
    {
        if(animator != null)
        {
            animator.SetTrigger("Remove");
        }

        yield return new WaitForSeconds(timeBeforeDissapearing);

        if(particleSystem != null)
            particleSystem.Play();

        ServiceLocator.GetScreenShake().StartScreenShake(2, 0.2f);
        ServiceLocator.GetScreenShake().StartScreenFlash(0.05f, 0.05f);
        collider.enabled = false;
        spriteRenderer.enabled = false;

        if (respawnAfterTime)
        {
            yield return new WaitForSeconds(respawnTime);
            Respawn();
        }
    }

    public void InstantRespawn()
    {
        if(particleSystem != null) 
            particleSystem.Stop();

        StopActiveCoroutine();
        Respawn();
    }

    private void Respawn()
    {
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }
        collider.enabled = true;
        spriteRenderer.enabled = true;
        removing = false;
    }


}
