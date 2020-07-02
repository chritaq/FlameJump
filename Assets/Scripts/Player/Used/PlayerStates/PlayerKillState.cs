using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillState : PlayerState
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float respawnTime;

    public override void Enter(PlayerController playerController)
    {
        ServiceLocator.GetScreenShake().StartScreenShake(50, 1f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(2, 0.1f);
        ServiceLocator.GetAudio().PlaySound("Player_Death", SoundType.interuptLast);
        ServiceLocator.GetScreenShake().StartScreenFlash(2, 0.3f);

        playerController.deathParticles.Play();
        playerController.redFlameParticles.SetActive(false);
        playerController.blueFlameParticles.SetActive(false);

        playerController.spriteAnimator.SetBool("Death", true);

        counterName = "Death";

        respawnTime = playerController.GetRespawnTime();
        //ServiceLocator.GetScreenShake().StartTransition((int)respawnTime * 30, true);

        playerController.canMove = false;

        //Start screenshake
        //Instantiate specific particleFX

        //Hides player sprite
        //spriteRenderer = playerController.GetComponent<SpriteRenderer>();
        //spriteRenderer.enabled = false;

        //Makes sure the player gets pinned to the place he died
        //Dosen't work!
        rb = playerController.AccessRigidBody();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        

    }

    public override void Exit(PlayerController playerController)
    {
        playerController.spriteAnimator.SetBool("Death", false);
        //ServiceLocator.GetScreenShake().StartTransition(25, false);

        rb.gravityScale = playerController.CheckInitialGravityScale();
        //Set playerpos to spawn
        playerController.transform.position = playerController.GetSpawnPosition();

        //Shows player sprite
        //spriteRenderer.enabled = true;

        playerController.canMove = true;

        playerController.gameManager.ResetStage();

    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    public override PlayerState Update(PlayerController playerController, float t)
    {
        rb.transform.transform.Translate(new Vector2(0, 0));
        //Timedelay for death
        respawnTime -= t;

        if(respawnTime <= 0)
        {
            return new PlayerIdleState();
        }

        return null;
    }

    
}
