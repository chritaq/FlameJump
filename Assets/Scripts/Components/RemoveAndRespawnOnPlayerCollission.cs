using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAndRespawnOnPlayerCollission : MonoBehaviour
{
    [SerializeField] private float respawnTime = 4f;
    private Collider2D collider;
    private SpriteRenderer sprite;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            StartCoroutine("RemoveAndRespawn");
            collision.GetComponent<PlayerController>().SetHealthAndDashChargesToMax();
        }
        
    }

    private IEnumerator RemoveAndRespawn()
    {
        collider.enabled = false;
        sprite.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        collider.enabled = true;
        sprite.enabled = true;
        yield return null;
    }
}
