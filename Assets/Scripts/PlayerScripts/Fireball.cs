using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 40; 
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.collider.CompareTag("Enemy"))
        {
            
            BasicEnemyController enemyScript = collision.transform.GetComponentInParent<BasicEnemyController>();
            RangeEnemyController rangeEnemyScript=collision.transform.GetComponentInParent<RangeEnemyController>();
            if (enemyScript != null)
            {
                
                
                enemyScript.TakeDamage(damage, transform.position.x);
            }
            if (rangeEnemyScript != null)
            {
                rangeEnemyScript.TakeDamage(damage, transform.position.x);
            }
            Destroy(gameObject); 
        }
    }
}

