using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableHidden : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool isHidden = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer == null || col == null)
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && !isHidden)
        {
            
            spriteRenderer.enabled = false;  
            isHidden = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && isHidden)
        {
            
            spriteRenderer.enabled = true;   
            isHidden = false;
        }
    }
}