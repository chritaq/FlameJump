﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    public GameObject redFlameParticles;
    public GameObject blueFlameParticles;
    public ParticleSystem deathParticles;
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
        if (health > 0)
        {
            health--;
        }
        else {
            returnedState = new PlayerKillState();
            StateSwap();
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
        //Lägg in i states istället?
        //if(health == 0)
        //{

        //    if(!redFlameParticles.activeSelf && dashCharges != 0)
        //    {
        //        redFlameParticles.SetActive(true);
        //        blueFlameParticles.SetActive(false);
        //    }
        //    if (!blueFlameParticles.activeSelf && dashCharges == 0)
        //    {
        //        blueFlameParticles.SetActive(true);
        //        redFlameParticles.SetActive(false);
        //    }
        //}
        //else if(health != 0)
        //{
        //    if(redFlameParticles.activeSelf)
        //    {
        //        redFlameParticles.SetActive(false);
        //    }
        //    if (blueFlameParticles.activeSelf)
        //    {
        //        blueFlameParticles.SetActive(false);
        //    }
            
        //}

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




}
