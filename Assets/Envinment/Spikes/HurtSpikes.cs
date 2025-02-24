using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HurtSpikes : MonoBehaviour
{
    public int damage = 100; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(playerHealth.maxHealth); 
            }
        }
    }
}