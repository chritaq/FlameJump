using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLockedInputState : PlayerState
{
    public override void Enter(PlayerController playerController)
    {
        playerController.AccessRigidBody().velocity = Vector2.zero;
        playerController.canMove = false;
    }

    public override void Exit(PlayerController playerController)
    {
    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    public override PlayerState Update(PlayerController playerController, float t)
    {
        return null;
    }
}
