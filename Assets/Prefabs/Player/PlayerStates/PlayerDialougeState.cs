using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialougeState : PlayerState
{
    public override void Enter(PlayerController playerController)
    {
        playerController.AccessRigidBody().velocity = Vector2.zero;
        playerController.canMove = false;

        //Starting should be done in the NPC?
        DialougeManagerV2.instance.StartDialouge(DialougeManagerV2.instance.testDialouge);
    }

    public override void Exit(PlayerController playerController)
    {
        playerController.canMove = true;
    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    public override PlayerState Update(PlayerController playerController, float t)
    {
        if(playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpHold)
        {
            DialougeManagerV2.instance.SpeedUpDialouge();
        }
        else
        {
            DialougeManagerV2.instance.SetDialougeSpeedToNormal();
        }

        //if(DialougeManagerV2.instance.sen)

        if(playerController.activeActionCommand == PlayerController.PlayerActionCommands.Dash)
        {
            if (DialougeManagerV2.instance.sentencesLeft <= 0)
            {
                DialougeManagerV2.instance.DisplayNextSentence();
                return new PlayerIdleState();
            }
            DialougeManagerV2.instance.DisplayNextSentence();
        }
        return null;
    }
}
