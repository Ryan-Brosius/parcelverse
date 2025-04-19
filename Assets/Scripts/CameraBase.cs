using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    
    [SerializeField] float zoom = 7.5f;
    
    private Vector3 refVelocity = Vector3.zero;
    [SerializeField] float smoothTime = 0.1f;

    private void Start()
    {
        GetComponent<Camera>().orthographicSize = zoom;
    }

    void LateUpdate()
    {
        Vector3 playerPosition = new(playerTarget.transform.position.x, playerTarget.transform.position.y, -10f);
        Vector3 mousePosition = GetComponent<Camera>().ScreenToWorldPoint(new(Input.mousePosition.x, 
            Input.mousePosition.y, -10f));
        
        // Calculating the player position twice for the average keeps the camera closer to the player than the mouse
        // cursor. This prevents the player from being too close to the edges of the screen.
        //transform.position = (playerPosition + playerPosition + mousePosition) / 3f;
        transform.position = Vector3.SmoothDamp(transform.position, 
            (playerPosition + playerPosition + mousePosition) / 3f, ref refVelocity, smoothTime);
    }
}
