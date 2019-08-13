using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ServiceLocator.Initialize();

        ServiceLocator.ProvideAudio(new NewAudioProvider());
        ServiceLocator.GetAudio().LoadSounds();
        ServiceLocator.GetAudio().PlaySound("Music_Gameplay01");

        ServiceLocator.ProvideScreenShake(new ScreenShakeProvider());
        ServiceLocator.GetScreenShake().GetCamera();

        ServiceLocator.ProvideGamepadRumble(new GamepadRumbleProvider());
        ServiceLocator.GetGamepadRumble().ReferenceGamepadRumble();
    }

    private void OnLevelWasLoaded(int level)
    {
        ServiceLocator.ProvideScreenShake(new ScreenShakeProvider());
        ServiceLocator.GetScreenShake().GetCamera();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
