using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCutsceneDialougeState : PlayerState
{
    private float skipTextStartTimer = 2.2f;
    private float skipTextTimer;
    private Coroutine skipTextTimerCoroutine;
    //private bool waitForHoldReleaseTrigger = false;

    private Rigidbody2D rb;

    public override void Enter(PlayerController playerController)
    {
        rb = playerController.AccessRigidBody();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        playerController.canMove = false;

        //playerController.gameManager.gameState = GameManager.GameState.cutscene;

        playerController.spriteAnimator.SetBool("Hide", true);
    }

    public override void Exit(PlayerController playerController)
    {
        playerController.canMove = true;
    }

    public override PlayerState FixedUpdate(PlayerController playerController, float t)
    {
        return null;
    }

    public override PlayerState Update(PlayerController playerController, float t)
    {
        if(playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpHold)
        {
            DialougeManagerV2.instance.SpeedUpDialouge();

            if (skipTextTimerCoroutine != null)
            {
                playerController.StopCoroutine(skipTextTimerCoroutine);
            }
            skipTextTimerCoroutine = playerController.StartCoroutine(StartSkipTextTimer());



        }
        else
        {
            DialougeManagerV2.instance.SetDialougeSpeedToNormal();
        }


        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpTap)
        {
            if (DialougeManagerV2.instance.endOfAnimations)
            {
                if(skipTextTimerCoroutine != null)
                {
                    playerController.StopCoroutine(skipTextTimerCoroutine);
                }

                skipTextTimer = 0;
                Debug.Log("Player is going to next sentence");
                if (DialougeManagerV2.instance.sentencesLeft <= 0)
                {
                    DialougeManagerV2.instance.DisplayNextSentenceV2();
                    return new PlayerCutsceneState();
                }
                DialougeManagerV2.instance.DisplayNextSentenceV2();

                //waitForHoldReleaseTrigger = true;
            }


            if (skipTextTimer > 0)
            {
                DialougeManagerV2.instance.QuicklySkipText();
                skipTextTimer = 0;
                
                Debug.Log("Quickly skipped text");
            }
        }


        if (playerController.activeActionCommand == PlayerController.PlayerActionCommands.Dash)
        {

            if(DialougeManagerV2.instance.endOfAnimations)
            {
                if (skipTextTimerCoroutine != null)
                {
                    playerController.StopCoroutine(skipTextTimerCoroutine);
                }
                skipTextTimer = 0;
                Debug.Log("Player is going to next sentence");
                if (DialougeManagerV2.instance.sentencesLeft <= 0)
                {
                    DialougeManagerV2.instance.DisplayNextSentenceV2();
                    return new PlayerCutsceneState();
                }
                DialougeManagerV2.instance.DisplayNextSentenceV2();
            }
            else
            {
                DialougeManagerV2.instance.QuicklySkipText();
            }

            
        }

        return null;
    }

    

    private IEnumerator StartSkipTextTimer()
    {
        skipTextTimer = skipTextStartTimer;
        while (skipTextTimer > 0)
        {
            skipTextTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
                
        }
        yield return null;
    }
}
