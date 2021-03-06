﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float initialGravityScale;
    private Rigidbody2D rb;
    private bool firstFrame = true;

    public override void Enter(PlayerController playerController)
    {
        playerController.heightAnimator.SetTrigger("Stretch");
        playerController.spriteAnimator.SetBool("JumpUp", true);

        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.small);
        ServiceLocator.GetAudio().PlaySound("Player_Jump", SoundType.interuptLast);

        counterName = "Jump";

        initialGravityScale = playerController.CheckInitialGravityScale();
        rb = playerController.AccessRigidBody();
        //rb.velocity = new Vector2(0, 0); //Shouold be in PlayerWallJumpState

        rb.velocity = new Vector2(rb.velocity.x, 0);

        //Jump
        rb.AddForce(Vector2.up * playerController.jumpVelocity, ForceMode2D.Impulse);
        playerController.StopLateJump();
    }


    public override void Exit(PlayerController playerController)
    {
        rb.gravityScale = initialGravityScale;
        playerController.spriteAnimator.SetBool("JumpUp", false);
        playerController.spriteAnimator.SetBool("Fall", false);
    }

    
    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        //Falls faster after height of jump
        if (rb.velocity.y < 0)
        {
            playerController.spriteAnimator.SetBool("Fall", true);
            playerController.spriteAnimator.SetBool("JumpUp", false);
            rb.gravityScale = playerController.fallMultiplier;
        }

        //Goes to lowjump if the player isn't pressing or holding down the jumpButton
        else if (rb.velocity.y > 0 && playerController.activeActionCommand != PlayerController.PlayerActionCommands.JumpHold && playerController.activeActionCommand != PlayerController.PlayerActionCommands.JumpTap)
        {
            playerController.CheckForRoofSpike();
            rb.gravityScale = playerController.lowJumpMultiplier;
        }

        else
        {
            if(rb.velocity.y > 0)
            {
                playerController.CheckForRoofSpike();
            }
            rb.gravityScale = initialGravityScale;
        }

        return null;
    }


    public override PlayerState Update(PlayerController playerController, float t)
    {
        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        if (playerController.CheckLateJump())
        {
            playerController.activeActionCommand = PlayerController.PlayerActionCommands.LateJump;
        }

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpTap || playerController.activeActionCommand == PlayerController.PlayerActionCommands.LateJump)
        {
            if (playerController.checkIfOnGround())
            {
                playerController.StopLateJump();
                return new PlayerJumpState();
            }
            else if(playerController.activeActionCommand != PlayerController.PlayerActionCommands.LateJump)
            {
                playerController.TryLateJump();
            }
        }

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Dash && playerController.dashCharges != 0)
        {
            return new PlayerDashState();
        }

        if (!firstFrame && playerController.checkIfOnGround() && (rb.velocity.y < 0 || rb.velocity.y == 0))
        {
            //Debug.Log("PlayedDust");
            //playerController.dustParticles.Play(true);
            return new PlayerIdleState();
        }

        firstFrame = false;

        return null;
    }





}
