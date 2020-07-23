using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSecondsComponent : MonoBehaviour
{
    [SerializeField] private float secondsBeforeDestroy = 1f;

    private void Start()
    {
        Destroy(this, secondsBeforeDestroy);
    }
}
