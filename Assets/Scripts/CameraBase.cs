using UnityEngine;

public class CameraBase : MonoBehaviour
{
    public Transform target;
    public float zoom = 7.5f;

    private void Start()
    {
        GetComponent<Camera>().orthographicSize = zoom;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10f);
    }
}
