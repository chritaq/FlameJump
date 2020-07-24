using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnCollission : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            particleSystem.Play();
        }
    }
}
