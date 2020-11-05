using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerOnCollision : MonoBehaviour
{
    public bool following = false;
    [SerializeField] private float speed = 1;

    public GameObject followOtherObject;

    // Update is called once per frame
    void Update()
    {
        if(following)
        {
            if(transformToFollow != null)
            {
                Vector2 dir = (transformToFollow.position - transform.position);
                transform.position += (new Vector3(dir.x, dir.y, 0) * speed);
            }
            else
            {
                following = false;
            }
        }
    }

    Transform transformToFollow;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!following && collision.tag == "Player")
        {
            following = true;
            if(followOtherObject != null)
            {
                transformToFollow = followOtherObject.transform;
            }
            else
            {
                transformToFollow = collision.transform;
            }

        }
    }

    public void SetNewFollowTransform(Transform newTransform)
    {
        transformToFollow = newTransform;
        following = true;
    }
}
