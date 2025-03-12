using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Knockback,
        Dead,
        Chasing,
        Attacking
    }

    private State currentState;

    [SerializeField]
    private Transform
         groundCheck,
         wallCheck;

    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [SerializeField] private Vector2 knockbackSpeed;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration,
        lastFlipTime,
        flipCooldown = 0.2f,
        detectionRange = 5f,
        attackRange = 1f,
        chargeSpeed = 3f,
        attackCooldown = 2f;
        

    private int facingDirection,
                damageDirection;

    private float currentHealth,
        knockbackStartTime,
        lastAttackTime;

    private Vector2 movement;

    private bool
        groundDetected,
        wallDetected;

    public GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;
    public Transform player;

    private void Start()
    {
        Transform aliveTransform = transform.Find("Alive");
        if (aliveTransform != null)
        {
            alive = aliveTransform.gameObject;
            aliveRb = alive.GetComponent<Rigidbody2D>();
            aliveAnim = alive.GetComponent<Animator>();
        }
        

        facingDirection = -1;
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;


    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
            case State.Chasing:
                UpdateChasingState();
                break;
            case State.Attacking:
                UpdateAttackingState();
                break;
        }
    }

    // WalkingState-----------------------------------------------------------------------
    private void UpdateMovingState()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(alive.transform.position, detectionRange, whatIsPlayer);

        if (playerHit != null && HasLineOfSight())
        {
            if ((player.position.x > alive.transform.position.x && facingDirection == -1) ||
                (player.position.x < alive.transform.position.x && facingDirection == 1))
            {
                Flip();
            }
            SwitchState(State.Chasing);
            return;
        }

        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, alive.transform.right, wallCheckDistance, whatIsGround);

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    // ChasingState---------------------------------------------------------------------- 
    private void UpdateChasingState()
    {
        if (player == null)
        {
            SwitchState(State.Moving);
            return;
        }

        Collider2D playerHit = Physics2D.OverlapCircle(alive.transform.position, detectionRange, whatIsPlayer);

        if (playerHit == null || !HasLineOfSight())
        {
            SwitchState(State.Moving);
            return;
        }

        float distanceToPlayer = Vector2.Distance(alive.transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            SwitchState(State.Attacking);
        }
        else
        {
            if ((player.position.x > alive.transform.position.x && facingDirection == -1) ||
                (player.position.x < alive.transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            movement.Set(chargeSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void UpdateAttackingState()
    {
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;  
            StartCoroutine(PerformAttack());  
        }
        else if (player == null || Vector2.Distance(alive.transform.position, player.position) > attackRange)
        {
           
            SwitchState(State.Chasing);
        }
    }

    private bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - alive.transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(alive.transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(alive.transform.position, directionToPlayer, distanceToPlayer, whatIsGround);

        return hit.collider == null; 
    }
    private IEnumerator PerformAttack()
    {
        aliveAnim.SetTrigger("Attack");

       
        Vector2 attackPosition = alive.transform.position;

       
        Debug.DrawLine(attackPosition, attackPosition + (Vector2)alive.transform.right * attackRange, Color.red, 1f);

        
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPosition, attackRange, whatIsPlayer);
        if (hitPlayers.Length > 0)
        {
            
            foreach (var hitPlayer in hitPlayers)
            {
                
                PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20);  
                }
            }
        }
       

        yield return new WaitForSeconds(attackCooldown); 
    }

    // KnockbackState------------------------------------------------------------------- 
    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void Flip()
    {
        if (Time.time - lastFlipTime > flipCooldown)
        {
            facingDirection *= -1;
            alive.transform.Rotate(0f, 180f, 0);
            lastFlipTime = Time.time;
        }
    }

    private void SwitchState(State state)
    {
        if (currentState != State.Dead)
        {
            currentState = state;
        }
    }

   
    public void TakeDamage(int damageAmount, float attackPositionX)
    {
        if (currentState == State.Dead)
            return;

        currentHealth -= damageAmount;

        aliveAnim.SetTrigger("Knockback");

        if (attackPositionX > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHealth > 0)
        {
            SwitchState(State.Knockback);
        }
        else
        {
            SwitchState(State.Dead);
        }
    }

    private void UpdateDeadState()
    {
        
        if (aliveAnim == null) return;

        aliveAnim.SetTrigger("Die");

        
        aliveRb.velocity = Vector2.zero;
        this.enabled = false; 

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(3f); 
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }


    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + alive.transform.right * wallCheckDistance);
    }
}