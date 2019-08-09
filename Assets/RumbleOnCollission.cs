using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleOnCollission : MonoBehaviour
{
    [SerializeField] private int vibrationTime = 25;
    [SerializeField] private float vibrationAmount = 0.5f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ServiceLocator.GetGamepadRumble().StartGamepadRumble(vibrationTime, vibrationAmount);
        }

    }
}
