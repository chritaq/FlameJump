using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExitState : PlayerState
{
    private SpriteRenderer spriteRenderer;

    public override void Enter(PlayerController playerController)
    {
        Debug.Log("Exited");
        playerController.canMove = false;

        ServiceLocator.GetScreenShake().StartScreenShake(150, 1f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(2, 1f);
        //ServiceLocator.GetAudio().PlaySound("Player_Exit");

        counterName = "Exit";

        //Hides player sprite
        spriteRenderer = playerController.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        //Need to pin the player to one place
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
