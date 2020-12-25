using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCutsceneState : PlayerState
{
    Rigidbody2D rb;

    public override void Enter(PlayerController playerController)
    {
        playerController.gameManager.gameState = GameManager.GameState.cutscene;
        rb = playerController.AccessRigidBody();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        playerController.spriteAnimator.SetBool("Hide", true);
        playerController.canMove = false;
    }

    public override void Exit(PlayerController playerController)
    {
        playerController.gameManager.gameState = GameManager.GameState.normal;
        rb.velocity = Vector2.zero;
        rb.gravityScale = playerController.CheckInitialGravityScale();
        playerController.spriteAnimator.SetBool("Hide", false);
        playerController.canMove = true;
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
