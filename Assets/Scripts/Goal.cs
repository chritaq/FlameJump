using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    [SerializeField] private int transitionTime;
    private ExitCommand exitCommand = new ExitCommand();

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform transform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            exitCommand.Excecute(collision.GetComponent<PlayerController>());
            StartCoroutine(Exit());
            ServiceLocator.GetScreenShake().StartTransition(transitionTime, true);
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
            transform.localScale = transform.localScale *= 1.1f;
            transitionTime--;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;
    }
}
