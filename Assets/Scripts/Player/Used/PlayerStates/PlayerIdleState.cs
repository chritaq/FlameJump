using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private bool grounded;

    public override void Enter(PlayerController playerController)
    {
        
    }


    public override void Exit(PlayerController playerController)
    {
        playerController.spriteAnimator.SetBool("Duck", false);
        playerController.spriteAnimator.SetBool("MovingGround", false);
    }


    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }


    public override PlayerState Update(PlayerController playerController, float t)
    {
        //Used to only set the bools to true if the player is grounded
        grounded = playerController.checkIfOnGround();

        if (playerController.activeVerticalCommand == PlayerController.PlayerVerticalCommands.Down && grounded)
        {
            playerController.heightAnimator.SetBool("Duck", grounded);
            playerController.spriteAnimator.SetBool("Duck", grounded);
        }
        else
        {
            playerController.heightAnimator.SetBool("Duck", false);
            playerController.spriteAnimator.SetBool("Duck", false);
        }

        if (playerController.activeHorizontalCommand == PlayerController.PlayerHorizontalCommands.Left || playerController.activeHorizontalCommand == PlayerController.PlayerHorizontalCommands.Right)
        {
            playerController.spriteAnimator.SetBool("MovingGround", grounded);
        }
        else
        {
            playerController.spriteAnimator.SetBool("MovingGround", false);
        }

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        ////TODO
        ////Add state for entering dialouge
        //Debug.Log("activeMiscCommand was: " + playerController.activeMiscCommand);
        //if(playerController.activeMiscCommand == PlayerController.PlayerMiscCommands.Dialouge)
        //{
        //    return new PlayerDialougeState();
        //}


        //FIRE PARTICLES
        if (playerController.GetPlayerHealth() != 0)
        {
            playerController.blueFlameParticles.SetActive(false);
            playerController.redFlameParticles.SetActive(false);
        }

        if (playerController.checkIfOnGround())
        {
            playerController.SetHealthAndDashChargesToMax();
        }


        if(playerController.CheckLateJump())
        {
            playerController.activeActionCommand = PlayerController.PlayerActionCommands.LateJump;
        } 


        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpTap || playerController.activeActionCommand == PlayerController.PlayerActionCommands.LateJump)
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

        if(!grounded)
        {
            Debug.Log("Went to fall state");
            return new PlayerFallState();
        }

        return null;
    }

    private IEnumerator LateJump()
    {
        yield return null;
    }
}
