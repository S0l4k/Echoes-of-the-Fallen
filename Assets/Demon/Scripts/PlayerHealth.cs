using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Animator anim;
    public Vector2 respawnPoint;  
    public float respawnTime = 3f;  
    public Rigidbody2D rb;
    private SpriteRenderer Sprite;

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        Sprite = rb.GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetTrigger("Die");

        
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        
        yield return new WaitForSeconds(0.8f);
        Sprite.enabled = false;
        
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {

        rb.velocity = new Vector2(0, 0);
        rb.simulated = false;
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(respawnTime);


        transform.position = respawnPoint;
        Sprite.enabled = true;
        currentHealth = maxHealth;
        transform.localScale = new Vector3(1, 1, 1);
        rb.simulated = true;




    }
}

