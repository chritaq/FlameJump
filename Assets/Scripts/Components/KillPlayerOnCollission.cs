using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnCollission : MonoBehaviour
{
    private PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerDeathCollider"))
        {
            //player = collision.GetComponent<PlayerController>();
            player = collision.GetComponentInParent<PlayerController>();
            player.Kill();
        }
    }
}
