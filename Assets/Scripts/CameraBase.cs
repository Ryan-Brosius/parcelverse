using UnityEngine;

public class CameraBase : MonoBehaviour
{
    private Transform playerTarget;
    
    [Range(1f, 10f)]
    [SerializeField] private float closenessToPlayer = 2f; // Higher values see the camera panning closer to the player.
    
    private Vector3 refVelocity = Vector3.zero;
    
    [Range(0f, 1f)]
    [SerializeField] private float smoothTime = 0.1f;

    private void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = new(playerTarget.transform.position.x, playerTarget.transform.position.y, -10f);
        Vector3 mousePosition = GetComponent<Camera>().ScreenToWorldPoint(new(Input.mousePosition.x, 
            Input.mousePosition.y, -10f));
        
        Vector3 targetPosition = ((playerPosition * closenessToPlayer) + mousePosition) /
                                 (2f + (closenessToPlayer - 1f));
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, smoothTime);
    }
}
