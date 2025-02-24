using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isFacingRight=true;
    private bool isGrounded;
    private bool isRunning;
    [SerializeField] bool canJump;
    
    private float movementInputDirection;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckRadius;

    public LayerMask whatIsGround;    
    
    private Rigidbody2D rb;
    private Animator anim;
    

    public Transform groundCheck;
    
    void Start()
    {
        rb= GetComponent<Rigidbody2D>(); 
        anim= GetComponent<Animator>();
    }

    
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }
    
   

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }
    private void CheckIfCanJump()
    {
        if(isGrounded&& rb.velocity.y<=0)
        {
            canJump = true;
        }
        else
        {
            canJump= false;
        }
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection <0 )
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0 ) 
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning= false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void ApplyMovement()
    {
        rb.velocity= new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);
    }

    private void Flip()
    {
        isFacingRight= !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0, 0f);
    }
    
    private void Jump()
    {
        if(canJump) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
