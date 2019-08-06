using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{


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
        if (health > 0)
        {
            health--;
        }
        else {
            returnedState = new PlayerKillState();
            StateSwap();
        }
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
    [Header("GROUND STUFF")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private float checkFloorRadius;
    [SerializeField] private LayerMask whatIsGround;


    public bool checkIfOnGround()
    {
        onGround = Physics2D.OverlapCircle(feetPos.position, checkFloorRadius, whatIsGround);
        if (onGround)
        {
            dashCharges = 1;
        }
        return onGround;
    }



    //Other
    [Header("Other")]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float respawnTime = 2f;

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

    [HideInInspector] public enum PlayerActionCommands { Nothing, Jump, JumpHold, Dash, LateJump };
    [HideInInspector] public PlayerActionCommands activeActionCommand;

    [HideInInspector] public enum PlayerHorizontalCommands { Nothing, Left, Right };
    [HideInInspector] public PlayerHorizontalCommands activeHorizontalCommand;
    [HideInInspector] public enum PlayerVerticalCommands { Nothing, Up, Down };
    [HideInInspector] public PlayerVerticalCommands activeVerticalCommand;


    private void Start()
    {
        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");

        //Puts player on spawnpoint
        transform.position = spawnPosition.transform.position;

        SetHealthAndDashChargesToMax();
        initialGravityScale = rb.gravityScale;
        
        currentState = new PlayerIdleState();
        currentState.Enter(this);
    }




    private void Update()
    {

        returnedState = currentState.Update(this, Time.deltaTime);

        if (returnedState != null)
        {
            StateSwap();
        }
    } 

    
    private void FixedUpdate()
    {
        //Returnedstate kan behövas här för att checka state-swaps vid physics-steps?
        //Just nu verkar den dock inte göra någonting?
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
                horizontalDirection = -1;
                break;
            case PlayerHorizontalCommands.Right:
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
    //private float activeSpeed = 0f;
    //[SerializeField]
    //private float turnAcceleration = 1f;
    //[SerializeField]
    //private float airTurnAcceleration = 1f;


    private void Movement()
    {
        direction = GetDirectionFromCommand();
        //direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        UpdateVerticalVelocity();

        //Check if the direction the player is moving is different from the input.
        //Tempcode for turning-speeds etc.
        //if(movement.x/activeSpeed != direction.x && direction.x != 0)
        //{
        //    activeSpeed = 0;
        //}

        //if (activeSpeed < maxMovementSpeed)
        //{
        //    if(!onGround)
        //    {
        //        activeSpeed = activeSpeed + airTurnAcceleration;
        //    }
        //    else
        //    {
        //        activeSpeed = activeSpeed + turnAcceleration;
        //    }
            
        //}

        movement = new Vector2(direction.x * maxMovementSpeed, verticalVelocity);


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
        //So we don't have two running at the same time
        StopCoroutine("LateJumpTimer");
        lateJump = false;

        StartCoroutine("LateJumpTimer");
    }

    public void StopLateJump()
    {
        StopCoroutine("LateJumpTimer");
        lateJump = false;
    }

    [SerializeField] private int lateJumpTimer = 30;
    private int timer;
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

}
