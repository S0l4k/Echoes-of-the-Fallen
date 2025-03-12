using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 15;
    public float lifetime = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);  // Usuwanie strza�y po up�ywie czasu
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzenie, czy strza�a trafi�a w gracza lub obiekt, kt�ry powinien j� zniszczy�
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerScript = collision.transform.GetComponentInParent<PlayerHealth>();
            if (playerScript != null)
            {
                Debug.Log("Strza�a trafi�a gracza!");
                playerScript.TakeDamage(damage);  // Wywo�anie metody obra�e�
            }
        }

        Destroy(gameObject);  // Zniszczenie strza�y po kolizji z czymkolwiek
    }
}