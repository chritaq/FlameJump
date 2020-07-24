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
    [HideInInspector] public bool unlocked;
    [HideInInspector] public int keysCollected;
    [HideInInspector] public static Goal instance;
    [SerializeField] private Sprite lockedSprite;
    private Sprite openSprite;

    private bool triggered = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(openSprite == null)
        {
            openSprite = spriteRenderer.sprite;
        }

        if (keysToUnlock > 0)
        {
            spriteRenderer.sprite = lockedSprite;
            unlocked = false;
        }
        else
        {
            unlocked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    public void AddKey()
    {
        keysCollected += 1;
        if(keysCollected >= keysToUnlock)
        {
            if(unlockDoorCoroutine != null)
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
        keysToUnlock++;
    }


    private IEnumerator UnlockDoor()
    {
        ServiceLocator.GetAudio().PlaySound("Player_Death", SoundType.interuptLast);
        ServiceLocator.GetTimeManagement().StopTimeforRealTimeSeconds(0.25f);
        yield return new WaitForSeconds(0.1f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);
        ServiceLocator.GetScreenShake().StartScreenShake(2, 0.2f);
        ServiceLocator.GetScreenShake().StartScreenFlash(0.1f, 1);
        ServiceLocator.GetAudio().PlaySound("Player_Bounce", SoundType.interuptLast);
        unlocked = true;
        spriteRenderer.sprite = openSprite;
    }
}
