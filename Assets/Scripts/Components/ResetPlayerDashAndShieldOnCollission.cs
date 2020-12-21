using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerDashAndShieldOnCollission : MonoBehaviour
{
    private PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PickupCollider")
        {
            playerController = collision.GetComponentInParent<PlayerController>();
            playerController.SetHealthAndDashChargesToMax();
        }
        
    }
}
