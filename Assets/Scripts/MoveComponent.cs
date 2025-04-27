using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;

    public bool affectedByGravity = true;
    
    public float jumpHeight = 2f;
    public float jumpGravity = -1f;
    public float fallGravity = -2f;

    public float acceleration = 1f;
    public float deceleration = 1f;

    public float unitScalar = 1f;
    
    private Vector2 moveDirection = new(0f, 0f);
    private Vector2 currentSpeed;
    public Vector2 maximumSpeed = new(10f, 999f);

    private Vector2 externalForce = Vector2.zero;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    
    private void Accelerate(ref float speedVar, float direction)
    {
        speedVar += (acceleration * unitScalar) * direction/* * Time.fixedDeltaTime*/;
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
    
    private void Decelerate(ref float speedVar)
    {
        speedVar += (deceleration * unitScalar) * Mathf.Sign(-speedVar)/* * Time.fixedDeltaTime*/;
        if (Mathf.Abs(speedVar) <= (deceleration * unitScalar))
        {
            speedVar = 0f;
        }
    }

    private void Decelerate2(ref float speedVar)
    {
        speedVar += (deceleration * unitScalar) * Mathf.Sign(-speedVar) * .5f/* * Time.fixedDeltaTime*/;
        if (Mathf.Abs(speedVar) <= (deceleration * unitScalar))
        {
            speedVar = 0f;
        }
    }

    public Vector2 GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsAscending()
    {
        return currentSpeed.y > 0f;
    }
    
    public bool IsDescending()
    {
        return currentSpeed.y < 0f;
    }
    
    private void Jump()
    {
        currentSpeed.y = Mathf.Abs(Mathf.Sqrt((jumpHeight * unitScalar) * -2f * (jumpGravity * 1f)));
    }

    public void SetCurrentSpeed(Vector2 newSpeed)
    {
        currentSpeed = newSpeed;
    }

    public void SetMoveDirection(Vector2 newMoveDirection)
    {
        moveDirection = newMoveDirection;
    }
    
    public void SetMoveDirectionToJump()
    {
        moveDirection = new(moveDirection.x, 1f);
    }

    public void AddExternalForce(Vector2 force)
    {
        externalForce += force;
    }

    public void Move(bool isGrounded = false)
    {
        if (Mathf.Abs(moveDirection.x) != 0f)
        {
            Accelerate(ref currentSpeed.x, moveDirection.x);

            if (Mathf.Abs(externalForce.x) > 0f)
            {
                Decelerate2(ref externalForce.x);
            }
        }
        else
        {
            if (Mathf.Abs(externalForce.x) > 0f)
            {
                Decelerate(ref externalForce.x);
            }
            else
            {
                Decelerate(ref currentSpeed.x);
            }
        }

        if (!affectedByGravity)
        {
            if (Mathf.Abs(moveDirection.y) != 0f)
            {
                Accelerate(ref currentSpeed.y, moveDirection.y);
            }
            else
            {
                Decelerate(ref currentSpeed.y);
            }
        }
        else
        {
            if (externalForce.y > 0f)
            {
                externalForce.y += (fallGravity * unitScalar);
                externalForce.y = Mathf.Max(0, externalForce.y);
            }
            else if (currentSpeed.y > 0f)
            {
                currentSpeed.y += (jumpGravity * unitScalar)/* * Time.fixedDeltaTime*/;
            }
            else
            {
                currentSpeed.y += (fallGravity * unitScalar)/* * Time.fixedDeltaTime*/;
            }
        }

        if (isGrounded)
        {
            currentSpeed.y = 0f;
        }

        if (moveDirection.y >= 1f && affectedByGravity && isGrounded)
        {
            Jump();
        }

        Cap(ref currentSpeed.x, maximumSpeed.x, moveDirection.x);
        if (affectedByGravity)
        {
            Cap(ref currentSpeed.y, maximumSpeed.y, -1f);
        }
        else
        {
            Cap(ref currentSpeed.y, maximumSpeed.y, moveDirection.y);
        }


        if (rigidbody2d != null)
        {
            //rigidbody2d.AddForce(currentSpeed/* * Time.fixedDeltaTime*/, ForceMode2D.Impulse);
            rigidbody2d.velocity = currentSpeed + externalForce;// * Time.fixedDeltaTime;
        }
    }
}