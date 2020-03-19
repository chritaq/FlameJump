using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void Enter(PlayerController playerController)
    {
        
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
        if(playerController.activeVerticalCommand == PlayerController.PlayerVerticalCommands.Down)
        {
            playerController.heightAnimator.SetBool("Duck", true);
        }
        else
        {
            playerController.heightAnimator.SetBool("Duck", false);
        }

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        //TODO
        //Add state for entering dialouge
        if(Input.GetKeyDown(KeyCode.U))
        {
            return new PlayerDialougeState();
        }


        //FIRE PARTICLES
        if (playerController.GetPlayerHealth() != 0)
        {
            playerController.blueFlameParticles.SetActive(false);
            playerController.redFlameParticles.SetActive(false);
        }


        playerController.CoyoteJumpTimer();

        if (playerController.checkIfOnGround())
        {
            playerController.SetHealthAndDashChargesToMax();
        }


        if(playerController.CheckLateJump())
        {
            playerController.activeActionCommand = PlayerController.PlayerActionCommands.LateJump;
        } 


        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Jump || playerController.activeActionCommand == PlayerController.PlayerActionCommands.LateJump)
        {
            if(playerController.checkIfOnGround() || playerController.CheckCoyoteJump())
            {
                playerController.StopLateJump();
                playerController.heightAnimator.SetBool("Duck", false);
                return new PlayerJumpState();
            }
            else if(playerController.activeActionCommand != PlayerController.PlayerActionCommands.LateJump)
            {
                playerController.TryLateJump();
            }
        }

        

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Dash && playerController.dashCharges != 0)
        {
            playerController.heightAnimator.SetBool("Duck", false);
            return new PlayerDashState();
        }

        return null;
    }

    private IEnumerator LateJump()
    {
        yield return null;
    }
}
