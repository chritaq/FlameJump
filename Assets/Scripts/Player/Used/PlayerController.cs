using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    //GameStuff
    [HideInInspector] public GameManager gameManager;

    public GameObject redFlameParticles;
    public GameObject blueFlameParticles;
    public ParticleSystem deathParticles;
    private Material startMaterial;
    private Material activeMaterial;
    [SerializeField] private float flashRate = 0.3f;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private Color flashColor;
    private Color activeNormalColor;
    private Color redStartColor;
    [SerializeField] private Color blueStartColor;
//public ParticleSystem dustParticles;


//Movement
    [Header("MOVEMENT")]
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float maxDownardSpeed;
    [HideInInspector] public bool canMove = true;
    private Vector2 direction;
    private float verticalVelocity;



    //Jump & Gravity
    [Header("JUMP & GRAVITY")]
    [Range(1, 15)]
    public float jumpVelocity;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    private float initialGravityScale;

    public float CheckInitialGravityScale()
    {
        return initialGravityScale;
    }



    //Dash
    [Header("DASH")]
    [SerializeField] private int maxDashCharges;
    [HideInInspector] public int dashCharges;
    [SerializeField] private float dashVelocity;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashStartDelay = 0.05f;

    public float GetDashVelocity()
    {
        return dashVelocity;
    }

    public float GetDashTime()
    {
        return dashTime;
    }

    public float GetDashStartDelay()
    {
        return dashStartDelay;
    }


    //Health, Burn and Bounce
    [Header("HEALTH, BURN & BOUNCE")]
    [SerializeField] private int maxHealth;
    private int health;
    public float bounceVelocity = 22f;

    public int GetPlayerHealth()
    {
        return health;
    }

    public override void Burn()
    {
        Debug.Log("Burned player");
        if (health > 0)
        {
            health--;
            StopFlash();
            flashPlayerCoroutine = StartCoroutine(FlashPlayerSprite());
        }
        else {
            Kill();
        }
    }

    public void Kill()
    {
        returnedState = new PlayerKillState();
        StateSwap();
    }



    
    public override void Bounce()
    {
        if (returnedState == null)
        {
            returnedState = new PlayerBounceState();
            StateSwap();
        }
    }


    public void SetHealthAndDashChargesToMax()
    {
        dashCharges = maxDashCharges;
        health = maxHealth;

        
    }



    //isGrounded
    private bool onGround = true;
    private bool onGroundLastFrame = true;
    [Header("GROUND STUFF")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private float checkFloorRadius;
    [SerializeField] private LayerMask whatIsGround;


    public bool checkIfOnGround()
    {
        onGround = Physics2D.OverlapCircle(feetPos.position, checkFloorRadius, whatIsGround);

        if (onGround)
        {
            if(!onGroundLastFrame)
            {
                StopFlash();

                ServiceLocator.GetGamepadRumble().StartGamepadRumble(1, 1f);
                ServiceLocator.GetAudio().PlaySound("Player_Land", SoundType.interuptLast);
                heightAnimator.SetTrigger("Squash");
                spriteAnimator.SetTrigger("Player_Land");
            }

            dashCharges = 1;
            onGroundLastFrame = true;
        }
        else
        {
            onGroundLastFrame = false;
        }

        return onGround;
    }



    //Other
    [Header("Other")]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float respawnTime = 2.6f;

    public Rigidbody2D AccessRigidBody()
    {
        return rb;
    }

    public float GetRespawnTime()
    {
        return respawnTime;
    }

    private GameObject spawnPosition;

    public Vector2 GetSpawnPosition()
    {
        return spawnPosition.transform.position;
    }



    //States & Commands
    PlayerState currentState;
    PlayerState returnedState;

    [HideInInspector] public enum PlayerActionCommands { Nothing, JumpTap, JumpHold, Dash, LateJump, Exit };
    [HideInInspector] public PlayerActionCommands activeActionCommand;

    [HideInInspector] public enum PlayerHorizontalCommands { Nothing, Left, Right };
    [HideInInspector] public PlayerHorizontalCommands activeHorizontalCommand;
    [HideInInspector] public enum PlayerVerticalCommands { Nothing, Up, Down };
    [HideInInspector] public PlayerVerticalCommands activeVerticalCommand;

    [HideInInspector] public enum PlayerMiscCommands {Nothing, Dialouge };
    [HideInInspector] public PlayerMiscCommands activeMiscCommand;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");

        //Puts player on spawnpoint
        transform.position = spawnPosition.transform.position;

        SetHealthAndDashChargesToMax();
        initialGravityScale = rb.gravityScale;
        
        currentState = new PlayerIdleState();
        currentState.Enter(this);

        startMaterial = playerSprite.material;
        redStartColor = playerSprite.color;
        activeNormalColor = redStartColor;
        SetTrailPool();
    }



    public Animator heightAnimator;
    public Animator spriteAnimator;
    [SerializeField] private SpriteRenderer playerSprite;

    private void Update()
    {
        CoyoteJumpTimer();

        returnedState = currentState.Update(this, Time.deltaTime);

        if (returnedState != null)
        {
            StateSwap();
        }

        ChooseColor();
    }

    private bool triggeredRed = false;
    private bool triggeredBlue = false;
    private void ChooseColor()
    {
        if (dashCharges <= 0) {
            activeNormalColor = blueStartColor;
            activeMaterial = flashMaterial;

            if (!triggeredBlue)
            {
                playerSprite.material = activeMaterial;
                playerSprite.color = activeNormalColor;
                triggeredBlue = true;
                triggeredRed = false;
            }
        }
        else
        {
            activeNormalColor = redStartColor;
            activeMaterial = startMaterial;
            if (!triggeredRed)
            {
                playerSprite.material = activeMaterial;
                playerSprite.color = activeNormalColor;
                triggeredRed = true;
                triggeredBlue = false;
            }
        }
    }

    
    private void FixedUpdate()
    {
        returnedState = currentState.FixedUpdate(this, Time.deltaTime);

        if (canMove)
        {
            Movement();
        }
    }

    
    private void StateSwap()
    {
        currentState.Exit(this);
        currentState = returnedState;
        currentState.Enter(this);
        if (returnedState.counterName != null)
        {
            Notify(returnedState.counterName, NotificationType.Counter);
        }
    }
    
    public Vector2 GetDirectionFromCommand()
    {
        float horizontalDirection = 0;
        switch(activeHorizontalCommand)
        {
            case PlayerHorizontalCommands.Left:
                playerSprite.flipX = false;
                horizontalDirection = -1;
                break;
            case PlayerHorizontalCommands.Right:
                playerSprite.flipX = true;
                horizontalDirection = 1;
                break;
        }

        float verticalDirection = 0;
        switch (activeVerticalCommand)
        {
            case PlayerVerticalCommands.Up:
                verticalDirection = 1;
                break;
            case PlayerVerticalCommands.Down:
                verticalDirection = -1;
                break;
        }

        return new Vector2(horizontalDirection, verticalDirection);
    }

    Vector2 movement;
    private float horizontalVelocity;
    [SerializeField] private float horizontalDamping;
    [SerializeField] private float acceleration;


    private void Movement()
    {
        direction = GetDirectionFromCommand();

        UpdateVerticalVelocity();

        //Code for turning-speed and acceleration
        if(direction.x != 0)
        {
            horizontalVelocity += direction.x * acceleration;
            //horizontalVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);
            horizontalVelocity = Mathf.Clamp(horizontalVelocity, -maxMovementSpeed, maxMovementSpeed);
            movement = new Vector2(horizontalVelocity, verticalVelocity);
        }
        else
        {
            if (horizontalVelocity < acceleration && horizontalVelocity > -acceleration)
            {
                horizontalVelocity = 0;
            }
            else
            {
                horizontalVelocity -= Mathf.Clamp(horizontalVelocity, -1, 1) * acceleration;
            }

            movement = new Vector2(horizontalVelocity, verticalVelocity);
        }

        UpdateVelocity();
        //rb.velocity = new Vector2(movement.x, movement.y);
    }


    private void UpdateVelocity()
    {
        rb.velocity = new Vector2(movement.x, movement.y);
    }

    private void UpdateVerticalVelocity()
    {

        if (rb.velocity.y < 0)
        {
            if (direction.y < 0)
            {
                verticalVelocity = Mathf.Clamp(rb.velocity.y * 2, -maxDownardSpeed * 2, maxDownardSpeed * 2);
            }
            else
            {
                verticalVelocity = Mathf.Clamp(rb.velocity.y, -maxDownardSpeed, maxDownardSpeed * 2);
            }
        }
        else
        {
            verticalVelocity = rb.velocity.y;
        }
    }


    private bool lateJump;
    public bool CheckLateJump()
    {
        return lateJump;
    }

    public void TryLateJump()
    {
        StopLateJump();
        lateJumpTimerCoroutine = StartCoroutine(LateJumpTimer());
    }

    public void StopLateJump()
    {
        if (lateJumpTimerCoroutine != null)
        {
            StopCoroutine(lateJumpTimerCoroutine);
        }
        lateJump = false;
    }

    [SerializeField] private int lateJumpTimer = 30;
    private int timer;
    private Coroutine lateJumpTimerCoroutine;
    private IEnumerator LateJumpTimer()
    {
        timer = lateJumpTimer;
        while(timer > 0)
        {
            timer--;
            lateJump = true;
            yield return new WaitForEndOfFrame();
        }
        lateJump = false;
        yield return null;
    }



    private int extraGroundedTimer;
    [SerializeField] private int coyoteJumpTime = 16;

    public void CoyoteJumpTimer()
    {
        if(onGround)
        {
            extraGroundedTimer = coyoteJumpTime;
        }
        extraGroundedTimer--;
    }

    public bool CheckCoyoteJump()
    {
        if(extraGroundedTimer > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void GoToDialougeState()
    {
        returnedState = new PlayerDialougeState();
        StateSwap();
    }

    public void GoToLockedInputState()
    {
        returnedState = new PlayerLockedInputState();
        StateSwap();
    }

    public PlayerState readCurrentPlayerState()
    {
        return currentState;
    }


    public Coroutine flashPlayerCoroutine;
    public IEnumerator FlashPlayerSprite() {
        Debug.Log("Started player flash");
        Debug.Log("Health is: " + health);
        bool flash = true;
        Color startColor = playerSprite.color;

        while (health <= 0) {
            if(flash)
            {
                playerSprite.material = flashMaterial;
                playerSprite.color = flashColor;
            }
            else
            {
                if(dashCharges <= 0)
                {
                    playerSprite.material = flashMaterial;
                    playerSprite.color = blueStartColor;
                }
                else
                {
                    playerSprite.material = startMaterial;
                    playerSprite.color = redStartColor;
                }

            }

            flash = !flash;
            yield return new WaitForSeconds(flashRate);
        }

        playerSprite.material = startMaterial;
        playerSprite.color = redStartColor;
    }

    public void StopFlash()
    {
        if(flashPlayerCoroutine != null)
        {
            StopCoroutine(flashPlayerCoroutine);
        }
        playerSprite.material = startMaterial;
        playerSprite.color = redStartColor;
    }

    [SerializeField] private float trailRate = 0.05f;
    [SerializeField] private GameObject trailObject;
    private Coroutine trailCoroutine;

    public void StartTrailCoroutine()
    {
        StopTrailCoroutine();
        trailCoroutine = StartCoroutine(DropTrail());
    }

    public void StopTrailCoroutine()
    {
        if (trailCoroutine != null)
        {
            StopCoroutine(trailCoroutine);
        }
    }

    GameObject tempTrailObject;
    SpriteRenderer tempTrailSpriteRenderer;
    private IEnumerator DropTrail()
    {
        while (true)
        {
            tempTrailObject = PoolTrailObject();
            if (tempTrailObject == null) break;
            tempTrailObject.SetActive(true);
            tempTrailSpriteRenderer = tempTrailObject.GetComponent<SpriteRenderer>();
            Debug.Log("Dropped trail!");

            tempTrailObject.transform.position = transform.position;
            tempTrailObject.transform.rotation = transform.rotation;

            if (playerSprite.flipX)
            {
                tempTrailSpriteRenderer.flipX = true;
            }
            tempTrailSpriteRenderer.sprite = playerSprite.sprite;
            yield return new WaitForSeconds(trailRate);
        }

    }

    private GameObject[] trailObjects;
    private int trailObjectLenght = 30;
    private void SetTrailPool()
    {
        trailObjects = new GameObject[trailObjectLenght];
        for (int i = 0; i < trailObjects.Length; i++)
        {
            trailObjects[i] = Instantiate(trailObject, null);
            trailObjects[i].SetActive(false);
        }
    }

    private GameObject PoolTrailObject()
    {
        for (int i = 0; i < trailObjects.Length; i++)
        {
            if (!trailObjects[i].activeSelf)
            {
                Debug.Log("Pooled object");
                return trailObjects[i];
            }
        }

        Debug.Log("Couldnt pool object");

        return null;
    }
}
