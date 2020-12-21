using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnNoHealth : MonoBehaviour
{
    private PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();

            if (player.GetPlayerHealth() == 0)
            {
                player.Burn();
            }
        }
    }
}
