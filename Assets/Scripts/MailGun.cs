using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailGun : MonoBehaviour
{
    public enum MailMode { Letter, Package }

    // Should prob be a proper input controllor in the future?
    // if not easy to change in future
    [Header("Input Keys")]
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode switchModeKey = KeyCode.E;

    // Projectiles the gun will fire
    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private GameObject packagePrefab;

    // Projectile settings
    [Header("Projectile Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float letterFireForce = 10f;
    [SerializeField] private float packageFireForce = 6f;

    // Create a trajectory to show player when aiming down
    [Header("Trajectory settings")]
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [SerializeField] private int maxTrajectorySteps = 100;
    [SerializeField] private float maxTrajectoryDistance = 10f;
    [SerializeField] private Material trajectoryMaterial;   //ehh should make this like a dotted line later please

    // I need an extra camera in the scene rn to actually raycast
    // raycasting is fucked up with the other cameras...
    // Ryan TODO fix this so it isnt a drag/drop later
    [Header("Other References")]
    [SerializeField] private Camera raycastCamera;
    [SerializeField] private Transform player;

    private MailMode currentMode = MailMode.Letter;
    private LineRenderer trajectoryLine;
    private Vector3 mouseWorldPos;
    private float localXPos;

    void Start()
    {
        trajectoryLine = GetComponent<LineRenderer>();
        if (trajectoryLine == null)
        {
            trajectoryLine = gameObject.AddComponent<LineRenderer>();
        }

        trajectoryLine.positionCount = 0;
        trajectoryLine.material = trajectoryMaterial != null ? trajectoryMaterial : new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.widthMultiplier = 0.05f;
        trajectoryLine.numCapVertices = 2;
        trajectoryLine.textureMode = LineTextureMode.Tile;

        localXPos = transform.localPosition.x;
    }

    void Update()
    {
        mouseWorldPos = raycastCamera.ScreenToWorldPoint(Input.mousePosition);

        PutCorrectSide();
        RotateTowardsMouse();
        HandleInput();
        HandleSlowMotion();
    }

    void PutCorrectSide()
    {
        if (mouseWorldPos.x > player.position.x) transform.localPosition = new Vector3(localXPos, transform.localPosition.y);
        else transform.localPosition = new Vector3(-localXPos, transform.localPosition.y);
    }

    void RotateTowardsMouse()
    {
        mouseWorldPos.z = transform.position.z;
        Vector3 direction = mouseWorldPos - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, Mathf.Infinity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(switchModeKey))
        {
            currentMode = currentMode == MailMode.Letter ? MailMode.Package : MailMode.Letter;
        }

        if (Input.GetKeyDown(fireKey))
        {
            FireProjectile();
        }
    }

    void HandleSlowMotion()
    {
        if (Input.GetKey(aimKey))
        {
            Time.timeScale = 0.5f;
            ShowTrajectory(currentMode == MailMode.Letter ? letterPrefab : packagePrefab);
        }
        else
        {
            Time.timeScale = 1f;
            trajectoryLine.positionCount = 0;
        }
    }

    void FireProjectile()
    {
        GameObject prefab = currentMode == MailMode.Letter ? letterPrefab : packagePrefab;
        GameObject projectile = Instantiate(prefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * (currentMode == MailMode.Letter ? letterFireForce : packageFireForce);
    }

    void ShowTrajectory(GameObject prefab)
    {
        Vector2 startPos = firePoint.position;
        Vector2 velocity = firePoint.right * (currentMode == MailMode.Letter ? letterFireForce : packageFireForce);
        float gravityScale;
        
        if (prefab.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            gravityScale = rb.gravityScale;
        }
        else
        {
            gravityScale = 1f;
        }

        List<Vector3> points = new List<Vector3>();

        points.Add(startPos);

        float simulatedGravity = Physics2D.gravity.y * gravityScale;
        float time = 0f;
        float distanceTraveled = 0f;

        Vector2 previousPoint = startPos;

        for (int i = 0; i < maxTrajectorySteps; i++)
        {
            time += trajectoryTimeStep;
            Vector2 newPoint;

            newPoint = startPos + velocity * time + 0.5f * new Vector2(0, simulatedGravity) * time * time;

            RaycastHit2D hit = Physics2D.Linecast(previousPoint, newPoint, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                points.Add(hit.point);
                break;
            }

            distanceTraveled += Vector2.Distance(previousPoint, newPoint);
            if (distanceTraveled >= maxTrajectoryDistance)
            {
                points.Add(newPoint);
                break;
            }

            points.Add(newPoint);
            previousPoint = newPoint;
        }

        trajectoryLine.positionCount = points.Count;
        trajectoryLine.SetPositions(points.ToArray());
    }
}
