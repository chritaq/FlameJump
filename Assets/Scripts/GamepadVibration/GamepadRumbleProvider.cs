using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadRumbleProvider : IGamepadRumbleService
{
    private GamepadRumbler gamepadRumbler;

    public void ReferenceGamepadRumble()
    {
        gamepadRumbler = GameObject.FindGameObjectWithTag("InputHandler").GetComponentInChildren<GamepadRumbler>();
    }

    public void StartGamepadRumble(int vibrationTime, float vibrationAmount)
    {
        gamepadRumbler.StartRumble(vibrationTime, vibrationAmount);
    }
}
