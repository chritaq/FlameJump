﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillState : PlayerState
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float respawnTime;

    public override void Enter(PlayerController playerController)
    {
        //playerController.respawnFreeze = true;
        ServiceLocator.GetScreenShake().StartScreenShake(10, 1f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);
        ServiceLocator.GetAudio().PlaySound("Player_Death", SoundType.interuptLast);
        ServiceLocator.GetScreenShake().StartScreenFlash(0.05f, 0.1f);

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

        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

    }

    public override void Exit(PlayerController playerController)
    {
        if(Goal.instance != null)
        {
            Goal.instance.ResetDoorAndKeys();
        }
        playerController.spriteAnimator.SetBool("Death", false);
        //ServiceLocator.GetScreenShake().StartTransition(25, false);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = playerController.CheckInitialGravityScale();
        //Set playerpos to spawn
        playerController.transform.position = playerController.GetSpawnPosition();

        //Shows player sprite
        //spriteRenderer.enabled = true;

        playerController.canMove = true;

        playerController.gameManager.ResetStage();

        playerController.StartCoroutine(playerController.RespawnFreeze());

        playerController.activeVerticalCommand = PlayerController.PlayerVerticalCommands.Nothing;
        playerController.activeHorizontalCommand = PlayerController.PlayerHorizontalCommands.Nothing;
        playerController.activeActionCommand = PlayerController.PlayerActionCommands.Nothing;
        playerController.activeMiscCommand = PlayerController.PlayerMiscCommands.Nothing;

    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    bool transitionTriggered = false;
    public override PlayerState Update(PlayerController playerController, float t)
    {
        rb.transform.transform.Translate(new Vector2(0, 0));
        //Timedelay for death
        respawnTime -= t;

        if (/*respawnTime <= playerController.GetRespawnTime() / 2 && */!transitionTriggered)
        {
            ServiceLocator.GetScreenShake().StartSwipe(true);
            transitionTriggered = true;
        }

        if(respawnTime <= 0)
        {
            ServiceLocator.GetScreenShake().StartSwipe(false);
            //ServiceLocator.GetScreenShake().StartTransition(25, false);
            return new PlayerIdleState();
        }

        return null;
    }

    
}
