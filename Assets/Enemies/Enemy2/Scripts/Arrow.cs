using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage = 15;
    public float lifetime = 2f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);  // Usuwanie strza³y po up³ywie czasu
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzenie, czy strza³a trafi³a w gracza lub obiekt, który powinien j¹ zniszczyæ
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerScript = collision.transform.GetComponentInParent<PlayerHealth>();
            if (playerScript != null)
            {
                Debug.Log("Strza³a trafi³a gracza!");
                playerScript.TakeDamage(damage);  // Wywo³anie metody obra¿eñ
            }
        }

        Destroy(gameObject);  // Zniszczenie strza³y po kolizji z czymkolwiek
    }
}