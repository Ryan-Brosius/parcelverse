using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    [SerializeField] private string identifier;
    
    private Rigidbody2D rigidbody2D;

    public bool affectedByGravity = true;
    
    public float jumpHeight = 2f;
    public float jumpGravity = -1f;
    public float fallGravity = -2f;

    public float acceleration = 1f;
    public float deceleration = 1f;
    
    private float currentSpeedX, currentSpeedY, currentSpeedZ;
    
    public Vector3 maximumSpeed = new(10f, 999f, 10f);

    private Vector3 moveDirection = new(0f, 0f, 0f);

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void Accelerate(ref float speedVar, float direction)
    {
        speedVar += (acceleration * 100f) * direction * Time.deltaTime;
    }

    private void Cap(ref float speedVar, float speedCap, float direction)
    {
        bool maxSpeedExceeded = Mathf.Abs(speedVar) > speedCap;
        bool maxSpeedWithDirectionalLimiterExceeded = (Mathf.Abs(speedVar) > speedCap * Mathf.Abs(direction)) &&
                                                       (direction != 0f);
        
        if (maxSpeedWithDirectionalLimiterExceeded)
        {
            speedVar = speedCap * direction;
        }
        else if (maxSpeedExceeded)
        {
            speedVar = speedCap * Mathf.Sign(direction);
        }
    }
    
    public void CopyCurrentSpeed(MoveComponent moverEnvied)
    {
        SetCurrentSpeed(moverEnvied.GetCurrentSpeed());
    }

    public void CopyMoveDirection(MoveComponent moverEnvied)
    {
        SetMoveDirection(moverEnvied.GetMoveDirection());
    }
    
    private void Decelerate(ref float speedVar)
    {
        speedVar += (deceleration * 100f) * Mathf.Sign(-speedVar) * Time.deltaTime;
        if (Mathf.Abs(speedVar) <= (deceleration * 100f))
        {
            speedVar = 0f;
        }
    }
    
    public Vector3 GetCurrentSpeed()
    {
        return new Vector3(currentSpeedX, currentSpeedY, currentSpeedZ);
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsAscending()
    {
        return currentSpeedY > 0f;
    }
    
    public bool IsDescending()
    {
        return currentSpeedY < 0f;
    }
    public void Bounce(float bounceHeight)
    {
        Debug.Log("Bounce applied! New height: " + bounceHeight);
        currentSpeedY = Mathf.Sqrt((bounceHeight * 100f) * -2f * (jumpGravity * 1f));
    }
    private void Jump()
    {
        currentSpeedY = Mathf.Abs(Mathf.Sqrt((jumpHeight * 100f) * -2f * (jumpGravity * 1f)));
    }

    public void MakeIntoProjectile()
    {
        moveDirection = transform.forward;
    }

    public void SetCurrentSpeed(Vector3 newSpeed)
    {
        currentSpeedX = newSpeed.x;
        currentSpeedY = newSpeed.y;
        currentSpeedZ = newSpeed.z;
    }
    
    public void SetHorizontalSpeedToMax()
    {
        currentSpeedX = moveDirection.x * maximumSpeed.x;
        currentSpeedZ = moveDirection.z * maximumSpeed.z;
    }

    public void SetMoveDirection(Vector3 newMoveDirection)
    {
        moveDirection = newMoveDirection;
    }
    
    public void SetMoveDirectionToJump()
    {
        moveDirection = new(moveDirection.x, 1f, moveDirection.z);
    }

    public void Move(bool isGrounded = false)
    {
        if (Mathf.Abs(moveDirection.x) != 0f)
        {
            Accelerate(ref currentSpeedX, moveDirection.x);
        }
        else
        {
            Decelerate(ref currentSpeedX);
        }

        if (!affectedByGravity)
        {
            if (Mathf.Abs(moveDirection.y) != 0f)
            {
                Accelerate(ref currentSpeedY, moveDirection.y);
            }
            else
            {
                Decelerate(ref currentSpeedY);
            }
        }

        if (Mathf.Abs(moveDirection.z) != 0f)
        {
            Accelerate(ref currentSpeedZ, moveDirection.z);
        }
        else
        {
            Decelerate(ref currentSpeedZ);
        }

        if (affectedByGravity)
        {
            if (currentSpeedY > 0f)
            {
                currentSpeedY += (jumpGravity * 100f) * Time.deltaTime;
            }
            else
            {
                currentSpeedY += (fallGravity * 100f) * Time.deltaTime;
            }
        }

        if (isGrounded)
        {
            currentSpeedY = 0f;
        }

        if (moveDirection.y >= 1f && affectedByGravity && isGrounded)
        {
            Jump();
        }

        Cap(ref currentSpeedX, maximumSpeed.x, moveDirection.x);
        if (affectedByGravity)
        {
            Cap(ref currentSpeedY, maximumSpeed.y, -1f);
        }
        else
        {
            Cap(ref currentSpeedY, maximumSpeed.y, moveDirection.y);
        }

        Cap(ref currentSpeedZ, maximumSpeed.z, moveDirection.z);

        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = new Vector2(currentSpeedX, currentSpeedY);// * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(currentSpeedX, currentSpeedY, 0f) * Time.deltaTime;
        }
    }
}