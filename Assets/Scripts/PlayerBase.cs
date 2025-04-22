using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MoveComponent))]

public class PlayerBase : MonoBehaviour
{
    [Header("Components")] public MoveComponent mover;

    [Header("Input")] public InputAction move;
    [SerializeField] InputAction jump;

    [HideInInspector] public Vector2 movementDirection;

    [SerializeField] Transform groundCheckTransform;
    [SerializeField] float groundCheckRadius = 0.5f;
    [SerializeField] LayerMask groundLayer;

    private float coyoteTimeTimer;
    [SerializeField] float coyoteTimeTimerSet = 0.1f;

    private float jumpBufferTimer;
    [SerializeField] float jumpBufferTimerSet = 0.1f;

    [SerializeField] private float jumpInterruptMinimumVelocity = 3f;

    public enum States
    {
        Idle,
        Walking,
        Jumping,
        Falling
    }

    [HideInInspector] public States currentState = States.Idle;
    private List<States> statesLog = new();

    void OnEnable()
    {
        move.Enable();
        jump.Enable();
    }

    void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    void Start()
    {
        Time.timeScale = 1f;
        statesLog.Add(currentState);
    }

    void Update()
    {
        movementDirection = Vector2.zero;
        ReceiveInput();

        switch (currentState)
        {
            case States.Idle:
                if (!IsGrounded())
                {
                    if (mover.IsAscending())
                    {
                        TransitionToJumping();
                    }
                    else
                    {
                        TransitionToFalling();
                    }
                }
                else if (jump.triggered && IsGrounded())
                {
                    TransitionToJumping();
                }
                else if (movementDirection != Vector2.zero)
                {
                    TransitionToWalking();
                }

                break;
            case States.Walking:
                if (!IsGrounded())
                {
                    if (mover.IsAscending())
                    {
                        TransitionToJumping();
                    }
                    else
                    {
                        TransitionToFalling();
                    }
                }
                else if (jump.triggered && IsGrounded())
                {
                    TransitionToJumping();
                }
                else if (movementDirection == Vector2.zero)
                {
                    TransitionToIdle();
                }

                break;
            case States.Jumping:
                coyoteTimeTimer -= Time.deltaTime;
                jumpBufferTimer -= Time.deltaTime;

                if (!mover.IsAscending())
                {
                    TransitionToFalling();
                }
                else if (!jump.IsPressed() && mover.GetCurrentSpeed().y >= jumpInterruptMinimumVelocity)
                {
                    mover.SetCurrentSpeed(new(mover.GetCurrentSpeed().x, jumpInterruptMinimumVelocity));
                    TransitionToFalling();
                }

                break;
            case States.Falling:
                coyoteTimeTimer -= Time.deltaTime;
                jumpBufferTimer -= Time.deltaTime;

                if (jump.IsPressed())
                {
                    jumpBufferTimer = jumpBufferTimerSet;
                }

                if (IsGrounded())
                {
                    if (jumpBufferTimer >= 0f)
                    {
                        jumpBufferTimer = 0f;
                        TransitionToJumping();
                    }
                    else if (movementDirection != Vector2.zero)
                    {
                        TransitionToWalking();
                    }
                    else
                    {
                        TransitionToIdle();
                    }
                }
                else
                {
                    if (jump.triggered && coyoteTimeTimer >= 0f)
                    {
                        coyoteTimeTimer = 0f;
                        TransitionToJumping();
                    }
                }

                break;
        }

        mover.SetMoveDirection(movementDirection);
    }

    private void FixedUpdate()
    {
        if (currentState == States.Jumping)
        {
            mover.Move(false);
        }
        else
        {
            mover.Move(IsGrounded());
        }
    }

    private States GetPreviousState()
    {
        int stateIndex = Mathf.Clamp(statesLog.Count - 1, 0, 999);
        return statesLog[stateIndex];
    }

    private void TransitionToIdle()
    {
        statesLog.Add(States.Idle);
        currentState = States.Idle;
    }

    private void TransitionToWalking()
    {
        statesLog.Add(States.Walking);
        currentState = States.Walking;
    }

    public void TransitionToJumping()
    {
        mover.SetMoveDirectionToJump();
        mover.Move(true);

        statesLog.Add(States.Jumping);
        currentState = States.Jumping;
    }

    private void TransitionToFalling()
    {
        statesLog.Add(States.Falling);
        currentState = States.Falling;
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }

    void ReceiveInput()
    {
        movementDirection = move.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        if (groundCheckTransform == null) return;

        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}