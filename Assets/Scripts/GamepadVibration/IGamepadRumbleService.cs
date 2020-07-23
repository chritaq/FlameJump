using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamepadRumbleService
{
    void ReferenceGamepadRumble();
    void StartGamepadRumble(GamepadRumbleProvider.RumbleSize rumbleSize);
}
