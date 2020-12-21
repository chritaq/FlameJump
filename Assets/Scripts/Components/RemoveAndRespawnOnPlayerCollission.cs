using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAndRespawnOnPlayerCollission : MonoBehaviour
{
    [SerializeField] private float respawnTime = 4f;
    private Collider2D collider;
    private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    [SerializeField] private bool respawnAfterPickup = true;

    [SerializeField] private ParticleSystem particleSystem;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private Coroutine respawnCoroutine;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PickupCollider")
        {
            if (respawnCoroutine != null) {
                StopCoroutine(respawnCoroutine);
            }

            respawnCoroutine = StartCoroutine("RemoveAndRespawn");
            //collision.GetComponent<PlayerController>().SetHealthAndDashChargesToMax();
        }
        
    }

    private IEnumerator RemoveAndRespawn()
    {
        collider.enabled = false;
        sprite.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Remove");
        }

        if (particleSystem != null)
            particleSystem.Play();

        if (respawnAfterPickup)
        {
            yield return new WaitForSeconds(respawnTime);
            collider.enabled = true;
            sprite.enabled = true;
            if (animator != null)
            {
                animator.SetTrigger("Respawn");
            }

            if (particleSystem != null)
                particleSystem.Stop();
        }


        yield return null;
    }

    public void InstantRespawn() {
        if(respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        collider.enabled = true;
        sprite.enabled = true;
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }

        if (particleSystem != null)
            particleSystem.Stop();
    }


}
