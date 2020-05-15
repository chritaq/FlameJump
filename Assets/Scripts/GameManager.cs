using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private bool debugSounds;

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
        ServiceLocator.GetScreenShake().StartTransition(100, false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
