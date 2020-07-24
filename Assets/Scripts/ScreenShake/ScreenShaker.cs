using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Vector3 startPosition;

    private void Start()
    {
        //cameraTransform = <Transform>();
        startPosition = new Vector3 (cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
    }

    public void StartScreenShake(float time, float amount)
    {
        //StopCoroutine(ScreenShake());
        //cameraTransform.position = startPosition;
        StartCoroutine(ScreenShake(time, amount));
        
    }

    private float positionX;
    private float positionY;
    private IEnumerator ScreenShake(float time, float amount)
    {
        startPosition = cameraTransform.position;
        //Start Screenshake
        while (time > 0)
        {
            positionX = Random.Range(-amount, amount) + startPosition.x;
            positionY = Random.Range(-amount, amount) + startPosition.y;
            time--;

            cameraTransform.position = new Vector3(positionX, positionY, cameraTransform.position.z);
            
            yield return new WaitForFixedUpdate();
        }
        //Stop Screenshake

        cameraTransform.position = startPosition;
        yield return null;
    }
}
