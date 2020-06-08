using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldWallGlideCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //private bool wallGliding = false;
    //[HideInInspector] public bool wallJumping = false;
    //private bool onWall = false;
    //private float checkWallRadius = 0.01f;
    //private Vector2 wallCollissionOffset = new Vector2(0.5f, 0);
    //[SerializeField] private float slideSpeed = 3f;

    //public bool checkIfOnWall()
    //{
    //    return onWall;
    //}

    //private void Slide()
    //{
    //    verticalVelocity = -slideSpeed;
    //}

    //private void Update()
    //{

    //    onWall = Physics2D.OverlapCircle((Vector2)transform.position + wallCollissionOffset, checkWallRadius, whatIsGround) && Input.GetKey(KeyCode.RightArrow)
    //    || Physics2D.OverlapCircle((Vector2)transform.position - wallCollissionOffset, checkWallRadius, whatIsGround) && Input.GetKey(KeyCode.LeftArrow);

    //    //Debug.Log(onWall);
    //    if (rb.velocity.y < 0 && activeActionCommand != PlayerActionCommands.Jump && onWall)
    //    {
    //        wallGliding = true;
    //    }
    //    else
    //    {
    //        wallGliding = false;
    //    }

    //    returnedState = currentState.Update(this, Time.deltaTime);
    //    CheckStateSwap();
    //}

    //private void Movement()
    //{
    //    direction = GetDirectionFromCommand();
    //    //direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    UpdateVerticalVelocity();
    //    movement = new Vector2(direction.x * movementSpeed, verticalVelocity);
    //    UpdateVelocity();
    //    //rb.velocity = new Vector2(movement.x, movement.y);
    //}

    //private void UpdateVelocity()
    //{
    //    if (!wallJumping)
    //    {
    //        rb.velocity = new Vector2(movement.x, movement.y);
    //    }
    //}

    //private void UpdateVerticalVelocity()
    //{
    //    if (wallGliding && !onGround)
    //    {
    //        Slide();
    //    }

    //    else if (rb.velocity.y < 0)
    //    {
    //        if (direction.y < 0)
    //        {
    //            verticalVelocity = Mathf.Clamp(rb.velocity.y * 2, -maxDownardSpeed * 2, maxDownardSpeed * 2);
    //        }
    //        else
    //        {
    //            verticalVelocity = Mathf.Clamp(rb.velocity.y, -maxDownardSpeed, maxDownardSpeed * 2);
    //        }
    //    }
    //    else
    //    {
    //        verticalVelocity = rb.velocity.y;
    //    }
    //}
}
