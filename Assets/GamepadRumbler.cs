using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GamepadRumbler : MonoBehaviour
{
    public void StartRumble(int vibrationTime, float vibrationAmount)
    {
        StopAllCoroutines();
        //GamePad.SetVibration(PlayerIndex.One, 0, 0);
        StartCoroutine(Rumbler(vibrationTime, vibrationAmount));

        
    }

    private IEnumerator Rumbler(int time, float amount)
    {
        //Start Rumble
        GamePad.SetVibration(PlayerIndex.One, amount, amount);
        while (time > 0)
        {
            time--;
            yield return new WaitForEndOfFrame();
        }
        //Stop Screenshake

        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        yield return null;
    }
}
