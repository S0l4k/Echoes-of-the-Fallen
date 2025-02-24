using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSlide : MonoBehaviour
{
    public Transform[] slidePoints;
    public float moveSpeed;
    public int slideDestination;

    private Vector2 lastPlatformPosition;
    private List<Rigidbody2D> objectsOnPlatform = new List<Rigidbody2D>();

    void Start()
    {
        lastPlatformPosition = transform.position;
    }

    void Update()
    {
        // Poruszanie platformy miêdzy punktami
        Vector2 targetPosition = slidePoints[slideDestination].position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            slideDestination = (slideDestination == 0) ? 1 : 0;
        }

        // Oblicz przesuniêcie platformy
        Vector2 movementDelta = (Vector2)transform.position - lastPlatformPosition;

        // Przesuñ ka¿dy obiekt na platformie o to samo przesuniêcie
        foreach (var obj in objectsOnPlatform)
        {
            if (obj != null) // Sprawdzamy, czy obiekt istnieje
            {
                obj.transform.position += (Vector3)movementDelta;
            }
        }

        // Zapisz aktualn¹ pozycjê platformy
        lastPlatformPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null && !objectsOnPlatform.Contains(rb))
        {
            objectsOnPlatform.Add(rb);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objectsOnPlatform.Remove(rb);
        }
    }
}
