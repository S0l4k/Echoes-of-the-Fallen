using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyController : MonoBehaviour
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
         wallCheck,
         arrowSpawnPoint;  

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
        attackRange = 10f,  
        chargeSpeed = 3f,
        attackCooldown = 2f,
        attackDelay = 0.5f;

    private int facingDirection;

   [SerializeField] private float currentHealth,
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

    public GameObject arrowPrefab; 
    public float arrowSpeed = 10f;
    public int arrowDamage = 15; 
    public float attackRate = 1.5f;
    private int damageDirection;

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

        SwitchState(State.Moving); 
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

    private void UpdateMovingState()
    {
        if (player != null && Vector2.Distance(alive.transform.position, player.position) <= detectionRange)
        {
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

    private void UpdateChasingState()
    {
        float distanceToPlayer = Vector2.Distance(alive.transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            SwitchState(State.Moving);  
            return;
        }

       

        
        if (distanceToPlayer > detectionRange)
        {
           
            SwitchState(State.Moving);
            return;
        }

        
        LookAtPlayer();

       
        if (distanceToPlayer <= attackRange)
        {
            SwitchState(State.Attacking);  
        }
        else
        {
            
            int direction = player.position.x > alive.transform.position.x ? 1 : -1;
            if (direction != facingDirection)
            {
                Flip();
            }

            movement.Set(chargeSpeed * direction, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }
    private void UpdateAttackingState()
    {
        if (player == null)
        {
            SwitchState(State.Moving);
            return;
        }

        float distanceToPlayer = Vector2.Distance(alive.transform.position, player.position);

        
        if (distanceToPlayer > detectionRange)
        {
           
            SwitchState(State.Moving);
            return;
        }

        LookAtPlayer();  

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformAttack());
        }
    }



    private IEnumerator PerformAttack()
    {
        if (player == null) yield break;  

        aliveAnim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);  

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

       
        Vector2 direction = (player.position - arrowSpawnPoint.position).normalized;
        rb.velocity = direction * arrowSpeed;

       
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


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

    private void LookAtPlayer()
    {
        if (player == null) return;

        float directionToPlayer = player.position.x - alive.transform.position.x;

       
       

       
        if (directionToPlayer > 0f && facingDirection < 0 || directionToPlayer < 0f && facingDirection > 0)
        {
            
            facingDirection *= -1;
            alive.transform.Rotate(0f, 180f, 0);  
            
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

        aliveAnim.SetTrigger("Hurt");

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + alive.transform.right * wallCheckDistance);
    }
}