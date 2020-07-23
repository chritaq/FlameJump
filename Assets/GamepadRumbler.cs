using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GamepadRumbler : MonoBehaviour
{
    private void OnLevelWasLoaded(int level)
    {
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
    }

    public void StartRumble(GamepadRumbleProvider.RumbleSize rumbleSize)
    {
        StopAllCoroutines();
        //GamePad.SetVibration(PlayerIndex.One, 0, 0);
        StartCoroutine(Rumbler(rumbleSize));

        
    }

    
    private IEnumerator Rumbler(GamepadRumbleProvider.RumbleSize rumbleSize)
    {
        //Start Rumble
        float motorOneAmount = 1;
        float motorTwoAmount = 1;
        float timeToRumble = 0;

        switch(rumbleSize)
        {
            case GamepadRumbleProvider.RumbleSize.small:
                timeToRumble = 0.05f;
                break;
            case GamepadRumbleProvider.RumbleSize.medium:
                timeToRumble = 0.1f;
                break;
            case GamepadRumbleProvider.RumbleSize.big:
                timeToRumble = 0.25f;
                break;
            case GamepadRumbleProvider.RumbleSize.huge:
                timeToRumble = 0.75f;
                break;
        }

        GamePad.SetVibration(PlayerIndex.One, motorOneAmount, motorTwoAmount);
        while (timeToRumble > 0)
        {
            timeToRumble -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Stop Screenshake

        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        yield return null;
    }
}
