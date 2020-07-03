using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    private Rigidbody2D rb;

    public override void Enter(PlayerController playerController)
    {
        rb = playerController.AccessRigidBody();
        playerController.spriteAnimator.SetBool("Fall", true);
        playerController.spriteAnimator.SetBool("JumpUp", false);
        rb.gravityScale = playerController.fallMultiplier;
    }

    public override void Exit(PlayerController playerController)
    {
        rb.gravityScale = playerController.CheckInitialGravityScale();
    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    private bool firstFrame = true;
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
            //Debug.Log("PlayedDust");
            //playerController.dustParticles.Play(true);
            return new PlayerIdleState();
        }

        firstFrame = false;

        return null;
    }
}
