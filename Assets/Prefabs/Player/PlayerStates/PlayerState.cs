using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public abstract void Enter(PlayerController playerController);
    public abstract void Exit(PlayerController playerController);
    public string counterName;

    //Dessa är av typen PlayerState då jag vill returna en PlayerState (exempelvis PlayerJumpState) i playerControllern.
    public abstract PlayerState Update(PlayerController playerController, float t);
    public abstract PlayerState FixedUpdate(PlayerController playerController, float t);

    //public abstract PlayerState HandleInput(PlayerController playerController)
}
