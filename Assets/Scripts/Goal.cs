using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    [SerializeField] private int transitionTime;
    private ExitCommand exitCommand = new ExitCommand();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            exitCommand.Excecute(collision.GetComponent<PlayerController>());
            StartCoroutine(Exit());
            ServiceLocator.GetScreenShake().StartTransition(transitionTime, true);
        }
    }

    private IEnumerator Exit()
    {
        while(transitionTime > 0)
        {
            transitionTime--;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield return null;
    }
}
