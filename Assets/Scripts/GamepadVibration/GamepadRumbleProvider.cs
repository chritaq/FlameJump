using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadRumbleProvider : IGamepadRumbleService
{
    private GamepadRumbler gamepadRumbler;

    public enum RumbleSize
    {
        small,
        medium,
        big,
        huge
    }

    public void ReferenceGamepadRumble()
    {
        gamepadRumbler = GameObject.FindGameObjectWithTag("InputHandler").GetComponentInChildren<GamepadRumbler>();
    }

    public void StartGamepadRumble(RumbleSize rumbleSize)
    {
        gamepadRumbler.StartRumble(rumbleSize);
    }
}
