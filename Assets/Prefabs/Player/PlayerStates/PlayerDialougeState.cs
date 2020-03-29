using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialougeState : PlayerState
{
    private float skipTextStartTimer = 2.2f;
    private float skipTextTimer;
    private Coroutine skipTextTimerCoroutine;
    //private bool waitForHoldReleaseTrigger = false;

    public override void Enter(PlayerController playerController)
    {
        playerController.AccessRigidBody().velocity = Vector2.zero;
        playerController.canMove = false;

        //Starting should be done in the NPC?
        //DialougeManagerV2.instance.StartDialouge(DialougeManagerV2.instance.testDialouge);
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
        //if(playerController.activeActionCommand != PlayerController.PlayerActionCommands.JumpHold)
        //{
        //    waitForHoldReleaseTrigger = false;
        //}


        if(playerController.activeActionCommand == PlayerController.PlayerActionCommands.JumpHold)
        {
            DialougeManagerV2.instance.SpeedUpDialouge();

            //if (!waitForHoldReleaseTrigger)
            //{
            //    if (skipTextTimerCoroutine != null)
            //    {
            //        playerController.StopCoroutine(skipTextTimerCoroutine);
            //    }
            //    skipTextTimerCoroutine = playerController.StartCoroutine(StartSkipTextTimer());
            //}

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
                    DialougeManagerV2.instance.DisplayNextSentence();
                    return new PlayerIdleState();
                }
                DialougeManagerV2.instance.DisplayNextSentence();

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
                    DialougeManagerV2.instance.DisplayNextSentence();
                    return new PlayerIdleState();
                }
                DialougeManagerV2.instance.DisplayNextSentence();
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
            Debug.Log("SkipTextTimer is: " + skipTextTimer);
            yield return new WaitForEndOfFrame();
                
        }
        yield return null;
    }
}
