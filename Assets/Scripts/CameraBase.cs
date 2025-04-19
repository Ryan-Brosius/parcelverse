using UnityEngine;

public class CameraBase : MonoBehaviour
{
    [SerializeField] Transform playerTarget;
    
    [SerializeField] float zoom = 7.5f;

    private void Start()
    {
        GetComponent<Camera>().orthographicSize = zoom;
    }

    void LateUpdate()
    {
        Vector2 playerPosition = playerTarget.transform.position;
        Vector2 mousePosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        
        // Calculating the player position twice for the average keeps the camera closer to the player than the mouse
        // cursor. This prevents the player from being too close to the edges of the screen.
        transform.position = (playerPosition + playerPosition + mousePosition) / 3f;
        transform.position += transform.forward * -10f;
    }
}
