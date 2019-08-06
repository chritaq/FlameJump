using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollission : MonoBehaviour
{
    [SerializeField]private string soundToPlay;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ServiceLocator.GetAudio().PlaySound(soundToPlay);
        }

    }
}
