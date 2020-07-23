using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounceState : PlayerState
{
    private float initialGravityScale;
    private Rigidbody2D rb;
    private bool firstFrame = true;

    public override void Enter(PlayerController playerController)
    {
        playerController.StartTrailCoroutine();
        playerController.heightAnimator.SetTrigger("Stretch");
        playerController.spriteAnimator.SetBool("JumpUp", true);

        ServiceLocator.GetScreenShake().StartScreenShake(2f, 0.2f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);
        ServiceLocator.GetAudio().PlaySound("Player_Bounce", SoundType.interuptLast);
        ServiceLocator.GetScreenShake().StartScreenFlash(2, 0.05f);
        counterName = "Bounce";

        //Gets the normal gravity for the player.
        initialGravityScale = playerController.CheckInitialGravityScale();

        //Makes sure the player dosen't have any vertical velocity at the start of the bounce.
        rb = playerController.AccessRigidBody();
        rb.velocity = new Vector2(rb.velocity.x, 0);

        //The force added for the bounce
        rb.AddForce(Vector2.up * playerController.bounceVelocity, ForceMode2D.Impulse);
        playerController.StopLateJump();
    }


    public override void Exit(PlayerController playerController)
    {
        playerController.StopTrailCoroutine();
        //Resets gravity for the player
        rb.gravityScale = initialGravityScale;
        playerController.redFlameParticles.SetActive(false);
        playerController.blueFlameParticles.SetActive(false);
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

        return null;
    }


    public override PlayerState Update(PlayerController playerController, float t)
    {
        if(playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        if(playerController.GetPlayerHealth() == 0 && rb.velocity.y > 0)
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
        else
        {
            playerController.redFlameParticles.SetActive(false);
            playerController.blueFlameParticles.SetActive(false);
        }
        


        if (playerController.CheckLateJump())
        {
            //playerController.redFlameParticles.SetActive(false);
            //playerController.blueFlameParticles.SetActive(false);
            playerController.activeActionCommand = PlayerController.PlayerActionCommands.LateJump;
        }

        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpTap || playerController.activeActionCommand == PlayerController.PlayerActionCommands.LateJump)
        {
            if (playerController.checkIfOnGround())
            {
                playerController.StopLateJump();
                playerController.redFlameParticles.SetActive(false);
                playerController.blueFlameParticles.SetActive(false);
                return new PlayerJumpState();
            }
            else if (playerController.activeActionCommand != PlayerController.PlayerActionCommands.LateJump)
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
            //playerController.dustParticles.Play(true);
            //Debug.Log("PlayedDust");
            return new PlayerIdleState();
        }

        firstFrame = false;

        return null;
    }

}
