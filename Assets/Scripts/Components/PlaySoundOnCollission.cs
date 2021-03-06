﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollission : MonoBehaviour
{
    [SerializeField] private string soundToPlay;
    [SerializeField] private SoundType interuptLast;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PickupCollider")
        {
            ServiceLocator.GetAudio().PlaySound(soundToPlay, interuptLast);
        }

    }
}
