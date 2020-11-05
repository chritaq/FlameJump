using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyComponent : MonoBehaviour
{
    private bool triggered = false;
    [SerializeField] private FollowPlayerOnCollision followPlayerOnCollision;
    private Collider2D collider;
    private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particleSystem;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        Goal.instance.AddKeysNeededForUnlock();
        collider = GetComponent<Collider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !triggered)
        {
            triggered = true;

            if (Goal.instance.keysCollected != 0)
            {
                followPlayerOnCollision.SetNewFollowTransform(Goal.instance.GetLatestKey().transform);
            }

            Goal.instance.AddKey(this);
            collider.enabled = false;
            //StartCoroutine(TriggeredDelay());
        }
    }

    //private IEnumerator TriggeredDelay()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    triggered = false;
    //}

    public void DestroyKey()
    {
        followPlayerOnCollision.following = false;
        ServiceLocator.GetScreenShake().StartScreenShake(4f, 0.4f);
        ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.big);

        ServiceLocator.GetAudio().PlaySound("Player_Bounce", SoundType.interuptLast);

        if (animator != null)
        {
            animator.SetTrigger("Remove");
        }

        if (particleSystem != null)
            particleSystem.Play();

        sprite.enabled = false;
    }

    public void InstantRespawn()
    {
        followPlayerOnCollision.following = false;
        transform.position = startPos;
        collider.enabled = true;
        sprite.enabled = true;
        triggered = false;
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }

        if (particleSystem != null)
            particleSystem.Stop();
    }

    //private IEnumerator TriggerOnPlayerGrounded()
    //{
    //    while(Goal.instance.playercon)
    //    {

    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}
