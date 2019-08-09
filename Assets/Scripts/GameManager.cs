﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Initialize();
        ServiceLocator.ProvideAudio(new NewAudioProvider());
        ServiceLocator.ProvideScreenShake(new ScreenShakeProvider());
        ServiceLocator.GetScreenShake().GetCamera();
        ServiceLocator.GetAudio().LoadSounds();
        ServiceLocator.GetAudio().PlaySound("Music_Gameplay01");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
