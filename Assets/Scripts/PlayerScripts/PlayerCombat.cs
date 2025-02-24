using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public Animator anim;

    public Transform attackPoint;
    public float attackRange = 0.5f;

    public LayerMask enemyLayers;

    // Attack 1
    public int attackDamage1 = 30;
    public float attackRate1 = 2f;

    // Attack 2
    public int attackDamage2 = 60;
    public float attackRate2 = 1f;

    // Attack 3 (fireball)
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    public int fireballDamage = 40;
    public float attackRate3 = 1.5f;

    private float nextAttackTime1 = 0f;
    private float nextAttackTime2 = 0f;
    private float nextAttackTime3 = 0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime1 && Input.GetKeyDown(KeyCode.H))
        {
            Attack1();
            nextAttackTime1 = Time.time + 1f / attackRate1;
        }

        if (Time.time >= nextAttackTime2 && Input.GetKeyDown(KeyCode.J))
        {
            Attack2();
            nextAttackTime2 = Time.time + 1f / attackRate2;
        }

        if (Time.time >= nextAttackTime3 && Input.GetKeyDown(KeyCode.K))
        {
            Attack3();
            nextAttackTime3 = Time.time + 1f / attackRate3;
        }
    }

    private void Attack1()
    {
        anim.SetTrigger("Attack1");
        DealDamage(attackDamage1);
    }

    private void Attack2()
    {
        anim.SetTrigger("Attack2");
        DealDamage(attackDamage2);
    }

    private void Attack3()
    {
        anim.SetTrigger("Attack3");
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * fireballSpeed;
    }

    private void DealDamage(int damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            BasicEnemyController basicEnemy = enemy.transform.parent?.GetComponent<BasicEnemyController>();
            RangeEnemyController rangeEnemy = enemy.transform.parent?.GetComponent<RangeEnemyController>();

            if (basicEnemy != null)
            {
                basicEnemy.TakeDamage(damage, transform.position.x);
            }
            else if (rangeEnemy != null)
            {
                rangeEnemy.TakeDamage(damage, transform.position.x);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
