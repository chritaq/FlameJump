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
        ServiceLocator.GetScreenShake().StartScreenShake(25, 0.5f);

        playerController.deathParticles.Play();
        playerController.redFlameParticles.SetActive(false);
        playerController.blueFlameParticles.SetActive(false);

        counterName = "Death";

        respawnTime = playerController.GetRespawnTime();

        playerController.canMove = false;

        //Start screenshake
        //Instantiate specific particleFX

        //Hides player sprite
        spriteRenderer = playerController.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        //Makes sure the player gets pinned to the place he died
        //Dosen't work!
        rb = playerController.AccessRigidBody();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        
    }

    public override void Exit(PlayerController playerController)
    {
        rb.gravityScale = playerController.CheckInitialGravityScale();
        //Set playerpos to spawn
        playerController.transform.position = playerController.GetSpawnPosition();

        //Shows player sprite
        spriteRenderer.enabled = true;

        playerController.canMove = true;

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
