using System.Collections;
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
    }

    public void AddKeysNeededForUnlock()
    {
        spriteRenderer.sprite = lockedSprite;
        unlocked = false;
        keysToUnlock++;
    }


    private IEnumerator UnlockDoor()
    {
        //while(PlayerController.inst)
        while (!playerController.CheckIfOnSafeGround())
        {
            yield return null;
        }

        for(int i = 0; i < keys.Count; i++)
        {
            keys[i].DestroyKey();
            yield return new WaitForSeconds(0.3f);
        }


        keys.Clear();

        ServiceLocator.GetAudio().PlaySound("Player_Death", SoundType.interuptLast);
        ServiceLocator.GetTimeManagement().StopTimeforRealTimeSeconds(0.25f);
        yield return new WaitForSeconds(0.1f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);
        ServiceLocator.GetScreenShake().StartScreenShake(2, 0.2f);
        ServiceLocator.GetScreenShake().StartScreenFlash(0.1f, 1);
        ServiceLocator.GetAudio().PlaySound("Player_Bounce", SoundType.interuptLast);
        spriteRenderer.sprite = openSprite;

        //Can use this delay so you'll get enough time to see the door open
        //yield return new WaitForSeconds(0.2f);

        unlocked = true;
    }
}
