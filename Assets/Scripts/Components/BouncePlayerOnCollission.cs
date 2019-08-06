using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlayerOnCollission : MonoBehaviour
{
    private PlayerController player;
    [SerializeField] private bool bounceOnlyWithHealth = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();

            if (bounceOnlyWithHealth == true && player.GetPlayerHealth() != 0)
            {
                player.Bounce();
            }

            else if(bounceOnlyWithHealth == false)
            {
                player.Bounce();
            }
            
        }
    }

}
