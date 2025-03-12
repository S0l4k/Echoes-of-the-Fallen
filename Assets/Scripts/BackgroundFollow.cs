using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cameraTransform;  
    public float parallaxFactorX = 0.5f;  
    public float parallaxFactorY = 0.5f;  

    private Vector3 previousCameraPosition;

    void Start()
    {
        
        previousCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        
        Vector3 cameraMovement = cameraTransform.position - previousCameraPosition;

        
        Vector3 newPosition = transform.position + new Vector3(cameraMovement.x * parallaxFactorX, cameraMovement.y * parallaxFactorY, 0);
        transform.position = newPosition;

        
        previousCameraPosition = cameraTransform.position;
    }
}