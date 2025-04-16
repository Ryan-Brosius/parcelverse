using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(HPComponent))]
[RequireComponent(typeof(MoveComponent))]

public class PlayerBase : MonoBehaviour
{
    [Header("Components")]
    //private HPComponent healthManager;
    
    public MoveComponent mover;
    public MoveComponent moverAir;

    [Header("Input")]
    public InputAction move;
    public InputAction jump;
    
    [HideInInspector] public float movementDirection;
    
    public Transform groundCheckTransform;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    private float coyoteTimeTimer;
    public float coyoteTimeTimerSet = 0.1f;
    
    private float jumpBufferTimer;
    public float jumpBufferTimerSet = 0.1f;

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

    private void Awake()
    {
        //healthManager = GetComponent<HPComponent>();
    }

    void Start()
    {
        Time.timeScale = 1f;
        
        statesLog.Add(currentState);
    }

    /*private void Update()
    {
        ReceiveInput();
    }*/

    void Update()
    {
        bool notOnGround = !IsGrounded();
        bool hasPerformedLegalJump = jump.triggered && IsGrounded();
        bool isMovingHorizontally = movementDirection != 0f;
        bool notMovingHorizontally = movementDirection == 0f;
        
        switch (currentState)
        {
            case States.Idle:
                ReceiveInput();
                
                mover.SetMoveDirection(Vector3.zero);
                mover.Move(IsGrounded());
                
                if (notOnGround)
                {
                    bool movingUpwards = mover.IsAscending();
                    if (movingUpwards)
                    {
                        TransitionToJumping();
                    }
                    else
                    {
                        TransitionToFalling();
                    }
                }
                else if (hasPerformedLegalJump)
                {
                    TransitionToJumping();
                }
                else if (isMovingHorizontally)
                {
                    TransitionToWalking();
                }

                break;
            case States.Walking:
                ReceiveInput();
                MoveOnGround();
                
                if (notOnGround)
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
                else if (hasPerformedLegalJump)
                {
                    TransitionToJumping();
                }
                else if (notMovingHorizontally)
                {
                    TransitionToIdle();
                }

                break;
            case States.Jumping:
                ReceiveInput();
                MoveInAir();
                
                coyoteTimeTimer -= Time.deltaTime;
                jumpBufferTimer -= Time.deltaTime;

                if (!moverAir.IsAscending())
                {
                    TransitionToFalling();
                }

                break;
            case States.Falling:
                ReceiveInput();
                MoveInAir();
                
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
                    else if (isMovingHorizontally)
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
    }

    #region StateTransitions

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
        if (GetPreviousState() == States.Jumping || GetPreviousState() == States.Falling)
        {
            mover.CopyCurrentSpeed(moverAir);
            mover.CopyMoveDirection(moverAir);
        }
        
        statesLog.Add(States.Walking);
        currentState = States.Walking;
    }
    
    public void TransitionToJumping()
    {
        moverAir.CopyCurrentSpeed(mover);
        if (GetPreviousState() == States.Idle || GetPreviousState() == States.Walking)
        {
            moverAir.CopyMoveDirection(mover);
        }
        moverAir.SetMoveDirectionToJump();
        //mover.SetMoveDirectionToJump();
        
        moverAir.Move(true);
        
        statesLog.Add(States.Jumping);
        currentState = States.Jumping;
    }
    
    private void TransitionToFalling()
    {
        coyoteTimeTimer = coyoteTimeTimerSet;
        
        if (GetPreviousState() == States.Walking)
        {
            moverAir.CopyCurrentSpeed(mover);
            moverAir.CopyMoveDirection(mover);
        }
        
        statesLog.Add(States.Falling);
        currentState = States.Falling;
    }

    #endregion

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }
    
    void ReceiveInput()
    {
        movementDirection = move.ReadValue<Vector2>().x;
    }
    
    void MoveOnGround()
    {
        Vector3 movementInput = Vector3.zero;
        
        movementInput = new(movementDirection, movementInput.y, 0f);

        mover.SetMoveDirection(movementInput);
        mover.Move(IsGrounded());
    }
    
    void MoveInAir()
    {
        Vector3 movementInput = Vector3.zero;
        
        movementInput = new(movementDirection, movementInput.y, 0f);

        moverAir.SetMoveDirection(movementInput);
        moverAir.Move(IsGrounded());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}