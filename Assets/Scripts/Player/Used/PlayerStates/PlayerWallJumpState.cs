using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    private float initialGravityScale;
    private Rigidbody2D rb;
    private bool jumpRequest;
    private bool firstFrame = true;

    public override void Enter(PlayerController playerController)
    {
        //playerController.wallJumping = true;
        //initialGravityScale = playerController.CheckInitialGravityScale();
        //rb = playerController.AccessRigidBody();
        //rb.velocity = new Vector2(0, 0);

        ////Jump
        //if(playerController.activeHorizontalCommand == PlayerController.PlayerHorizontalCommands.Left)
        //{
        //    rb.AddForce(new Vector2(playerController.jumpVelocity, playerController.jumpVelocity), ForceMode2D.Impulse);
        //}

        //if(playerController.activeHorizontalCommand == PlayerController.PlayerHorizontalCommands.Right)
        //{
        //    rb.AddForce(new Vector2(-playerController.jumpVelocity, playerController.jumpVelocity), ForceMode2D.Impulse);
        //}

        //rb.AddForce(Vector2.up * playerController.jumpVelocity, ForceMode2D.Impulse);
    }


    public override void Exit(PlayerController playerController)
    {
        //rb.gravityScale = initialGravityScale;
        //playerController.wallJumping = false;
    }


    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        ////Falls faster after height of jump
        //if (rb.velocity.y < 0)
        //{
        //    playerController.wallJumping = false;
        //    rb.gravityScale = playerController.fallMultiplier;
        //}

        ////Goes to lowjump if the player isn't pressing or holding down the jumpButton
        //else if (rb.velocity.y > 0 && playerController.activeActionCommand != PlayerController.PlayerActionCommands.JumpHold && playerController.activeActionCommand != PlayerController.PlayerActionCommands.Jump)
        //{
        //    playerController.wallJumping = false;
        //    rb.gravityScale = playerController.lowJumpMultiplier;
        //}

        //else
        //{
        //    rb.gravityScale = initialGravityScale;
        //}

        return null;
    }


    public override PlayerState Update(PlayerController playerController, float t)
    {
        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Exit)
        {
            return new PlayerExitState();
        }

        //if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Dash && playerController.dashCharges != 0)
        //{
        //    return new PlayerDashState();
        //}

        //if (!firstFrame && playerController.checkIfOnGround() && (rb.velocity.y < 0 || rb.velocity.y == 0))
        //{
        //    return new PlayerIdleState();
        //}

        //if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Jump && playerController.checkIfOnWall())
        //{
        //    return new PlayerWallJumpState();
        //}

        //Debug.Log("WallJumpState");

        //firstFrame = false;

        return null;
    }
}
