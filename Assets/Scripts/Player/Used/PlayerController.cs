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
    [SerializeField] private GameObject dustParticles;
    public ParticleSystem[] dashParticles;
    public TrailRenderer trailRenderer;


    //Movement
    [Header("MOVEMENT")]
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float maxMovementSpeedAir = 10f;
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

    public void GoToCutsceneState()
    {
        returnedState = new PlayerCutsceneState();
        StateSwap();
    }

    public void GoToIdleState()
    {
        returnedState = new PlayerIdleState();
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
    [SerializeField] private Transform feetPos2;
    [SerializeField] private float checkFloorRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsSafeGround;

    private GameObject spawnedDustParticle;
    public bool checkIfOnGround()
    {
        onGround = Physics2D.Linecast(feetPos.position, feetPos2.position, whatIsGround);
        if (onGround)
        {
            //Collider2D hit = Physics2D.OverlapCircle(feetPos.position, checkFloorRadius, whatIsGround);
            RaycastHit2D hit = Physics2D.Linecast(feetPos.position, feetPos2.position, whatIsGround);
            if (hit != null && hit.collider.GetComponent<TimedRemoveAfterCollission>())
            {
                hit.collider.GetComponent<TimedRemoveAfterCollission>().StartRemoveAfterCollision();
            }

            if (!onGroundLastFrame)
            {
                
                spawnedDustParticle = Instantiate(dustParticles, heightAnimator.transform.position, heightAnimator.transform.rotation, null);
                spawnedDustParticle.transform.localScale = heightAnimator.transform.localScale;
                spawnedDustParticle.GetComponentInChildren<ParticleSystem>().Play();
                StopFlash();

                ServiceLocator.GetGamepadRumble().StartGamepadRumble(GamepadRumbleProvider.RumbleSize.small);
                ServiceLocator.GetAudio().PlaySound("Player_Land", SoundType.interuptLast);
                heightAnimator.SetTrigger("Squash");
                //spriteAnimator.SetTrigger("Player_Land");
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

    //Hack
    public bool CheckIfOnGround(bool dontChangeDashCharges)
    {
        int startCharges = dashCharges;
        
        bool onGroundBool = checkIfOnGround();

        if (dontChangeDashCharges)
        {
            dashCharges = startCharges;
        }

        return onGroundBool;
    }

    public bool CheckIfOnSafeGround()
    {
        return Physics2D.Linecast(feetPos.position, feetPos2.position, whatIsSafeGround);
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

    public static PlayerController playerInstance;

    private void Awake()
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
        }
        playerInstance = this;

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {

        activeActionCommand = PlayerActionCommands.Nothing;
        activeHorizontalCommand = PlayerHorizontalCommands.Nothing;
        activeMiscCommand = PlayerMiscCommands.Nothing;

        StartCoroutine(RespawnFreeze());


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

        if (respawnFreeze || gameManager.gameState == GameManager.GameState.cutscene) return;

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
        if (respawnFreeze || gameManager.gameState == GameManager.GameState.cutscene) return;

        returnedState = currentState.FixedUpdate(this, Time.deltaTime);
        if (canMove)
        {
            Movement();
        }
           
    }

    public bool respawnFreeze = false;
    public IEnumerator RespawnFreeze()
    {
        Debug.Log("Respawn freeze started");
        respawnFreeze = true;
        float delay = 0.25f;
        while(delay > 0)
        {
            Debug.Log("Respawn freeze running");
            delay -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        respawnFreeze = false;
        yield return null;
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
    [SerializeField] private float airAcceleration;
    private float lastDirectionX = 0;

    private Vector2 posLastFrame;

    private void Movement()
    {
        direction = GetDirectionFromCommand();

        //Debug.Log(direction.x);

        UpdateVerticalVelocity();

        //Code for turning-speed and acceleration
        if(!onGround && direction.x != 0)
        {
            if(direction.x == lastDirectionX || lastDirectionX == 0)
            {
                horizontalVelocity += direction.x * airAcceleration;
            }
            else
            {
                //This is so when the player is turning. Should fix code so it's the same for turn/de-acceleration below.
                horizontalVelocity += direction.x * airAcceleration * 1.5f;
            }
            
            //horizontalVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);
            horizontalVelocity = Mathf.Clamp(horizontalVelocity, -maxMovementSpeedAir, maxMovementSpeedAir);
            movement = new Vector2(horizontalVelocity, verticalVelocity);
        }
        else if(direction.x != 0)
        {
            if (direction.x == lastDirectionX || lastDirectionX == 0)
            {
                horizontalVelocity += direction.x * acceleration;
            }
            else
            {
                //This is so when the player is turning. Should fix code so it's the same for turn/de-acceleration below.
                horizontalVelocity += direction.x * acceleration * 1.5f;
            }
            //horizontalVelocity *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);
            horizontalVelocity = Mathf.Clamp(horizontalVelocity, -maxMovementSpeed, maxMovementSpeed);
            movement = new Vector2(horizontalVelocity, verticalVelocity);
        }
        else
        {
            if(!onGround)
            {
                Debug.Log("velocity before deacceleration: " + horizontalVelocity);
                Debug.Log("rigibody before deacceleration: " + rb.velocity.x);
                //Det här blir fel eftersom den utgår efter spelarens command
                if (rb.velocity.x > 1 || rb.velocity.x < -1f)
                {
                    horizontalVelocity -= lastDirectionX * airAcceleration * 1.5f;
                    //horizontalVelocity = Mathf.Clamp(verticalVelocity, -1, 1);
                }
                else
                {
                    horizontalVelocity = 0;
                }
            }
            else
            {
                if (rb.velocity.x > 1 || rb.velocity.x < -1f)
                {
                    horizontalVelocity -= lastDirectionX * acceleration * 1.5f;
                    //horizontalVelocity = Mathf.Clamp(verticalVelocity, -1, 1);
                }
                else
                {
                    horizontalVelocity = 0;
                }
            }

            //Debug.Log(movement.x);

            


            movement = new Vector2(horizontalVelocity, verticalVelocity);
        }

        //Hack, just so it wont be null. Shuld probably do nullref instead.
        if(posLastFrame == null)
        {
            posLastFrame = rb.position;
        }

 
        if (movement.x > 0)
        {
            StopAtWallCollision(wallChecks[2].position, wallChecks[3].position);
        }
        else if (movement.x < 0)
        {
            StopAtWallCollision(wallChecks[0].position, wallChecks[1].position);
        }

        posLastFrame = rb.position;

        UpdateVelocity();
        //rb.velocity = new Vector2(movement.x, movement.y);
    }



    [SerializeField] private Transform[] wallChecks;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask spikeLayerMask;
    private void UpdateVelocity()
    {
        if(rb.velocity.x > 0)
        {
            lastDirectionX = 1;
        }
        else if(rb.velocity.x < 0)
        {

            lastDirectionX = -1;
        }
        else
        {
            lastDirectionX = 0;
        }

        rb.velocity = new Vector2(movement.x, movement.y);
    }

    private void StopAtWallCollision(Vector2 wallColliderHi, Vector2 wallColliderLow)
    {
        RaycastHit2D[] wallHits = Physics2D.LinecastAll(wallColliderHi, wallColliderLow, wallLayerMask);
        Debug.Log("Checking for wallhit");
        if (wallHits.Length > 0)
        {
            Debug.Log("Hit wall");
            horizontalVelocity = 0;
            movement = new Vector2(0, movement.y);
            RaycastHit2D spikeHit = Physics2D.Linecast(this.transform.position, new Vector2(wallColliderHi.x, transform.position.y), spikeLayerMask);
            if(spikeHit)
            {
                Debug.Log("Spike not hit");
                Kill();
            }

        }
    }

    [SerializeField] private Transform roofCheck;
    public void CheckForRoofSpike()
    {
        RaycastHit2D spikeHit = Physics2D.Linecast(this.transform.position, new Vector2(transform.position.x, roofCheck.position.y), spikeLayerMask);
        if (spikeHit)
        {
            Debug.Log("Spike not hit");
            Kill();
        }
    }


    private float downwardAcceleration = 0.1f;
    private void UpdateVerticalVelocity()
    {

        if (rb.velocity.y < 0)
        {
            if (direction.y < 0)
            {
                verticalVelocity += direction.y * airAcceleration * 1.5f;
                verticalVelocity = Mathf.Clamp(verticalVelocity, -maxDownardSpeed * 2, maxDownardSpeed * 2);
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

    //LATE JUMP AND DASH

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

    [SerializeField] private float lateJumpTimer = 0.25f;
    private float timer;
    private Coroutine lateJumpTimerCoroutine;
    private IEnumerator LateJumpTimer()
    {
        timer = lateJumpTimer;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            lateJump = true;
            yield return new WaitForFixedUpdate();
        }
        lateJump = false;
        yield return null;
    }


    private bool lateDash;
    public bool CheckLateDash()
    {
        return lateDash;
    }

    public void TryLateDash()
    {
        StopLateDash();
        lateDashTimerCoroutine = StartCoroutine(LateDashTimer());
    }

    public void StopLateDash()
    {
        if (lateDashTimerCoroutine != null)
        {
            StopCoroutine(lateDashTimerCoroutine);
        }
        lateDash = false;
    }

    [SerializeField] private float lateDashTime = 0.25f;
    private float lateDashTimer;
    private Coroutine lateDashTimerCoroutine;
    private IEnumerator LateDashTimer()
    {
        lateDashTimer = lateDashTime;
        while (timer > 0)
        {
            lateDashTimer -= Time.deltaTime;
            lateDash = true;
            yield return new WaitForFixedUpdate();
        }
        lateDash = false;
        yield return null;
    }




    private float extraGroundedTimer;
    [SerializeField] private float coyoteJumpTime = 0.5f;

    public void CoyoteJumpTimer()
    {
        if(onGround)
        {
            extraGroundedTimer = coyoteJumpTime;
        }
        extraGroundedTimer -= Time.deltaTime;
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

    public void GoToCutsceneDialougeState()
    {
        returnedState = new PlayerCutsceneDialougeState();
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

            tempTrailObject.transform.position = transform.position;
            tempTrailObject.transform.rotation = transform.rotation;

            tempTrailSpriteRenderer.flipX = playerSprite.flipX;

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
                return trailObjects[i];
            }
        }

        Debug.Log("Couldnt pool object");

        return null;
    }
}
