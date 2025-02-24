using UnityEngine;

public class CatFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    public float distanceThreshold = 0.5f;
    public float teleportThreshold = 50f; 
    public float jumpHeight = 5f;
    public float jumpSpeed = 5f;
    private bool isWalking;

    private bool isJumping = false;
    private float jumpStartHeight;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        FollowPlayer();
        HandleTeleportation();
        HandleJump();
    }

    private void FollowPlayer()
    {
        
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        if (Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

           
            isWalking = true;
        }
        else
        {
            
            isWalking = false;
        }

        
        anim.SetBool("isWalking", isWalking);
        FlipTowardsPlayer();
    }
    private void HandleJump()
    {
        
        if (player.GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            if (!isJumping)
            {
                isJumping = true;
                jumpStartHeight = transform.position.y;
            }

            
            transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, player.position.y + jumpHeight, jumpSpeed * Time.deltaTime), transform.position.z);
        }
        
    }
    private void HandleTeleportation()
    {
        
        if (Vector3.Distance(transform.position, player.position) > teleportThreshold)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
    private void FlipTowardsPlayer()
    {
        
        if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}