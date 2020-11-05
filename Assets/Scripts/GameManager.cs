﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private bool debugSounds;
    private RemoveAndRespawnOnPlayerCollission[] removeAndRespawnObjects;
    private KeyComponent[] keyObjects;
    private TimedRemoveAfterCollission[] timedRemoveObjects;

    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Initialize();

        ServiceLocator.ProvideAudio(new AudioProvider2(debugSounds));
        ServiceLocator.GetAudio().LoadSounds();

        ServiceLocator.ProvideScreenShake(new ScreenShakeProvider());
        ServiceLocator.GetScreenShake().GetCamera();

        ServiceLocator.ProvideGamepadRumble(new GamepadRumbleProvider());
        ServiceLocator.GetGamepadRumble().ReferenceGamepadRumble();

        ServiceLocator.ProvideTimeManagement(new TimeManagementProvider());
        ServiceLocator.GetTimeManagement().InstantiateTimeManagement();

        removeAndRespawnObjects = FindObjectsOfType<RemoveAndRespawnOnPlayerCollission>();
        keyObjects = FindObjectsOfType<KeyComponent>();
        timedRemoveObjects = FindObjectsOfType<TimedRemoveAfterCollission>();
        //ServiceLocator.ProvideScreenOverlay(new ScreenOverlayProvider());
        //ServiceLocator.GetScreenOverlay().ReferenceScreenOverlay();
    }

    private void OnLevelWasLoaded(int level)
    {
        //Fullösning, måste fixas!
        if(level == 1)
        {
            ServiceLocator.GetAudio().PlayMusic("Music_Gameplay01");
        }

        ServiceLocator.ProvideScreenShake(new ScreenShakeProvider());
        ServiceLocator.GetScreenShake().GetCamera();
        //Gör så en fadein görs i början av varje scen. Bör inte vara här?
        if(level != 0)
        {
            ServiceLocator.GetScreenShake().StartSwipe(false);
        }

        //StartTransition(100, false);

        removeAndRespawnObjects = FindObjectsOfType<RemoveAndRespawnOnPlayerCollission>();
        timedRemoveObjects = FindObjectsOfType<TimedRemoveAfterCollission>();
        keyObjects = FindObjectsOfType<KeyComponent>();
    }

    public void ResetStage() {
        for (int i = 0; i < removeAndRespawnObjects.Length; i++) {
            removeAndRespawnObjects[i].InstantRespawn();
        }
        for(int i = 0; i < timedRemoveObjects.Length; i++)
        {
            timedRemoveObjects[i].InstantRespawn();
        }
        for (int i = 0; i < keyObjects.Length; i++)
        {
            keyObjects[i].InstantRespawn();
        }
    }

    // Update is called once per frame
    void Update()

    {
    }
}
