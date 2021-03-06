﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    [SerializeField] private float transitionTime;
    private ExitCommand exitCommand = new ExitCommand();

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform transform;
    private int keysToUnlock;
    [HideInInspector] public bool unlocked = true;
    [HideInInspector] public int keysCollected;
    [HideInInspector] public static Goal instance;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite openSprite;

    private bool triggered = false;

    private PlayerController playerController;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Unlocked was:" + unlocked);
        if (collision.tag == "Player" && !triggered && unlocked)
        {
            triggered = true;
            exitCommand.Excecute(collision.GetComponent<PlayerController>());
            StartCoroutine(Exit());
            //ServiceLocator.GetScreenShake().StartTransition(transitionTime, true);
            ServiceLocator.GetTimeManagement().SlowDown(0.01f, true);
            ServiceLocator.GetAudio().PlaySound("Environment_DoorTransition", SoundType.interuptLast);
        }
    }

    private IEnumerator Exit()
    {
        spriteRenderer.sortingOrder = 100;
        ServiceLocator.GetScreenShake().StartSwipe(true);
        while (transitionTime > 0)
        {
            transform.localScale = transform.localScale += transform.localScale * Time.deltaTime * 20;
            transitionTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;
    }

    private List<KeyComponent> keys = new List<KeyComponent>();
    public void AddKey(KeyComponent newKey)
    {
        keys.Add(newKey);
        keysCollected += 1;
        TryUnlockDoor();
    }

    public KeyComponent GetLatestKey()
    {
        return keys[keys.Count - 1];
    }

    public void TryUnlockDoor()
    {
        if (keysCollected >= keysToUnlock && !unlocked)
        {
            if (unlockDoorCoroutine != null)
            {
                StopCoroutine(unlockDoorCoroutine);
            }
            
            unlockDoorCoroutine = StartCoroutine(UnlockDoor());
        }
    }

    private Coroutine unlockDoorCoroutine;
    public void ResetDoorAndKeys()
    {
        if(unlockDoorCoroutine != null)
        {
            StopCoroutine(unlockDoorCoroutine);
        }
        if (keysToUnlock > 0)
        {
            unlocked = false;
            spriteRenderer.sprite = lockedSprite;
        }
        keysCollected = 0;
        keys.Clear();
        doorAnimator.SetBool("Shake", false);
        doorEffectAnimator.SetBool("FadeIn", false);
        doorEffectAnimator.SetBool("FadeOut", false);
    }

    public void AddKeysNeededForUnlock()
    {
        spriteRenderer.sprite = lockedSprite;
        unlocked = false;
        keysToUnlock++;
    }

    [SerializeField] private Animator doorEffectAnimator;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float distanceNeededForOpenDoor = 2f;
    private IEnumerator UnlockDoor()
    {
        //while(PlayerController.inst)
        while (!(playerController.CheckIfOnSafeGround() && Vector2.Distance(this.transform.position, playerController.transform.position) < distanceNeededForOpenDoor))
        {
            Debug.Log("Distance was: " + Vector2.Distance(this.transform.position, playerController.transform.position));
            yield return null;
        }
        Debug.Log("Distance was: " + Vector2.Distance(this.transform.position, playerController.transform.position));

        float keyExplosionDelay = 0.3f;
        float startExplosionDelay = keyExplosionDelay;
        for(int i = 0; i < keys.Count; i++)
        {
            keys[i].DestroyKey();

            //if(i == keys.Count - 1)
            //{
            //    Doorflash.SetActive(true);
            //}
            keyExplosionDelay -= 0.08f;
            if(keyExplosionDelay < 0.05)
            {
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                yield return new WaitForSeconds(keyExplosionDelay);
            }

        }

        keyExplosionDelay = startExplosionDelay;


        keys.Clear();
        doorEffectAnimator.SetBool("FadeIn", true);
        yield return new WaitForSecondsRealtime(0.11f);
        doorAnimator.SetBool("Shake", true);
        ServiceLocator.GetAudio().PlaySound("Player_Death", SoundType.interuptLast);
        ServiceLocator.GetTimeManagement().StopTimeforRealTimeSeconds(0.5f);
        yield return new WaitForSeconds(0.1f);
        doorEffectAnimator.SetBool("FadeOut", true);
        doorEffectAnimator.SetBool("FadeIn", false);
        doorAnimator.SetBool("Shake", false);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);
        //ServiceLocator.GetScreenShake().StartScreenFlash(0.1f, 1);
        ServiceLocator.GetScreenShake().StartScreenShake(10f, 0.2f);
        ServiceLocator.GetAudio().PlaySound("Player_Bounce", SoundType.interuptLast);
        spriteRenderer.sprite = openSprite;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.sortingOrder = 0;


        

        //Can use this delay so you'll get enough time to see the door open
        //yield return new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(0.1f);

        unlocked = true;
    }
}
