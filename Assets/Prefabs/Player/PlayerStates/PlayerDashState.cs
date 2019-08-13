﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Rigidbody2D rb;
    private float dashStartDelay;
    private bool dashRequest;
    private float dashTime;

    public override void Enter(PlayerController playerController)
    {
        

        ServiceLocator.GetAudio().PlaySound("Player_Dash");
        counterName = "Dash";

        rb = playerController.AccessRigidBody();
        TurnOffGravity();

        playerController.dashCharges--;
        playerController.canMove = false;

        dashStartDelay = playerController.GetDashStartDelay();
        dashRequest = true;
        dashTime = playerController.GetDashTime();

        //FIRE PARTICLES
        if (playerController.GetPlayerHealth() == 0)
        {
            if (playerController.dashCharges != 0)
            {
                playerController.redFlameParticles.SetActive(true);
                playerController.blueFlameParticles.SetActive(false);
            }
            else
            {
                playerController.blueFlameParticles.SetActive(true);
                playerController.redFlameParticles.SetActive(false);
            }
        }

    }


    private void TurnOffGravity()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }


    public override void Exit(PlayerController playerController)
    {

        TurnOnGravity(playerController);
        playerController.canMove = true;
    }


    private void TurnOnGravity(PlayerController playerController)
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = playerController.CheckInitialGravityScale();
    }


    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    
    public override PlayerState Update(PlayerController playerController, float t)
    {
        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        dashStartDelay -= t;

        if (dashRequest == true && dashStartDelay <= 0)
        {
            ServiceLocator.GetGamepadRumble().StartGamepadRumble(25, 0.5f);
            ServiceLocator.GetScreenShake().StartScreenShake(dashTime, 0.2f);
            AddDashVelocityOnce(playerController);
        }

        if (playerController.checkIfOnGround() && playerController.activeActionCommand == PlayerController.PlayerActionCommands.Jump)
        {
            return new PlayerJumpState();
        }

        return TryEndDash(t);
    }


    private void AddDashVelocityOnce(PlayerController playerController)
    {
        rb.velocity = playerController.GetDirectionFromCommand().normalized * playerController.GetDashVelocity();
        dashRequest = false;
    }


    private PlayerState TryEndDash(float t)
    {
        if (!dashRequest)
        {
            dashTime -= t;
            if (dashTime <= 0)
            {
                return new PlayerIdleState();
            }
        }

        return null;
    }
}