using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPlayerOnCollission : MonoBehaviour
{
    private PlayerController player;

    //Will only burn the player if the player has health left (is this even necessary?)
    [SerializeField] private bool burnOnlyWithHealth = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();

            if (burnOnlyWithHealth)
            {
                if (player.GetPlayerHealth() != 0)
                {
                    player.Burn();
                }
            }

            else
            {
                player.Burn();
            }
        }
    }
}
