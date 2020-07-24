using System.Collections;
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
        playerController.heightAnimator.SetTrigger("Expand");
        //Enter dash, choose correct animation
        //if(playerController.activeHorizontalCommand != PlayerController.PlayerHorizontalCommands.Nothing)

        ServiceLocator.GetAudio().PlaySound("Player_Dash", SoundType.interuptLast);
        counterName = "Dash";

        rb = playerController.AccessRigidBody();
        TurnOffGravity();

        playerController.dashCharges--;
        playerController.canMove = false;

        dashStartDelay = playerController.GetDashStartDelay();
        dashRequest = true;
        dashTime = playerController.GetDashTime();



        //FIRE PARTICLES
        //if (playerController.GetPlayerHealth() == 0)
        //{
        //    if (playerController.dashCharges != 0)
        //    {
        //        playerController.redFlameParticles.SetActive(true);
        //        playerController.blueFlameParticles.SetActive(false);
        //    }
        //    else
        //    {
        //        playerController.blueFlameParticles.SetActive(true);
        //        playerController.redFlameParticles.SetActive(false);
        //    }
        //}

    }


    private void TurnOffGravity()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
    }


    public override void Exit(PlayerController playerController)
    {
        playerController.StopTrailCoroutine();
        TurnOnGravity(playerController);
        playerController.canMove = true;
        if (!playerController.checkIfOnGround())
        {

            //if (playerController.GetPlayerHealth() <= 0)
            //{
            //    playerController.spriteAnimator.SetBool("BurnedNoDash", true);
            //}
            //else if (playerController.dashCharges <= 0)
            //{
            //    playerController.spriteAnimator.SetBool("NoDash", true);
            //}
        }


        playerController.spriteAnimator.SetBool("DashUp", false);
        playerController.spriteAnimator.SetBool("DashSides", false);
        playerController.spriteAnimator.SetBool("DashDiagonal", false);
        playerController.trailRenderer.time = 0f;
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
            playerController.StartTrailCoroutine();
            ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.small);
            ServiceLocator.GetScreenShake().StartScreenShake(dashTime, 0.2f);
            AddDashVelocityOnce(playerController);
        }

        if (playerController.checkIfOnGround() && playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpTap)
        {
            return new PlayerJumpState();
        }

        return TryEndDash(t, playerController);
    }

    Vector2 directionOfDash;
    private void AddDashVelocityOnce(PlayerController playerController)
    {
        playerController.trailRenderer.time = 0.2f;
        directionOfDash = playerController.GetDirectionFromCommand().normalized;
        rb.velocity = directionOfDash * playerController.GetDashVelocity();
        if(playerController.activeVerticalCommand != PlayerController.PlayerVerticalCommands.Nothing && playerController.activeHorizontalCommand != PlayerController.PlayerHorizontalCommands.Nothing)
        {
            playerController.heightAnimator.SetTrigger("Squeeze");
            playerController.spriteAnimator.SetBool("DashDiagonal", true);
        }
        else if(playerController.activeVerticalCommand != PlayerController.PlayerVerticalCommands.Nothing)
        {

            if(directionOfDash.y > 0)
            {
                playerController.dashParticles[0].Play();
            }
            else if (directionOfDash.y < 0)
            {
                playerController.dashParticles[1].Play();
            }

            playerController.spriteAnimator.SetBool("DashUp", true);
            playerController.heightAnimator.SetTrigger("Stretch");
        }
        else if(playerController.activeHorizontalCommand != PlayerController.PlayerHorizontalCommands.Nothing)
        {
            if (directionOfDash.x < 0)
            {
                playerController.dashParticles[2].Play();
            }
            else if (directionOfDash.x > 0)
            {
                playerController.dashParticles[3].Play();
            }
            playerController.spriteAnimator.SetBool("DashSides", true);
            playerController.heightAnimator.SetTrigger("Squash");
        }
        //if((rb.velocity.x > 0 || rb.velocity.x < 0) && rb.velocity.y == 0)
        //{
        //    playerController.heightAnimator.SetTrigger("Squash");
        //}
        //else if((rb.velocity.y > 0 || rb.velocity.y < 0) && rb.velocity.x == 0)
        //{
        //    playerController.heightAnimator.SetTrigger("Stretch");
        //}
        dashRequest = false;
    }

    float forceOfExtraMovementAtEnd = 100;
    private PlayerState TryEndDash(float t, PlayerController playerController)
    {
        if (!dashRequest)
        {
            dashTime -= t;
            if (dashTime <= 0)
            {
                if (playerController.checkIfOnGround())
                {
                    Debug.Log("Went to idle state");
                    return new PlayerIdleState();
                }
                else
                {
                    playerController.AccessRigidBody().AddForce(Vector2.up * forceOfExtraMovementAtEnd);
                    if (directionOfDash.y > 0)
                    {
                        playerController.AccessRigidBody().AddForce(Vector2.up * forceOfExtraMovementAtEnd);

                    }
                    Debug.Log("Went to Fall state");
                    return new PlayerFallState();
                }

            }
        }

        return null;
    }
}
