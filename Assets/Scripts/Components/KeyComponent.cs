using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyComponent : MonoBehaviour
{
    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        Goal.instance.AddKeysNeededForUnlock();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !triggered)
        {
            triggered = true;
            Goal.instance.AddKey();
            StartCoroutine(TriggeredDelay());
        }
    }

    private IEnumerator TriggeredDelay()
    {
        yield return new WaitForSeconds(0.5f);
        triggered = false;
    }

}
