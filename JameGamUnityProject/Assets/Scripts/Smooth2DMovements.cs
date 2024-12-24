using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Smooth2DMovements : MonoBehaviour
{
    [Header("---------- INPUTS ----------")]
    public InputActionReference move;
    public InputActionReference jump;

    [Header("---------- WALK ----------")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;

    [Header("---------- RUN ----------")]
    [Range(1f, 100f)] public float maxRunSpeed = 20f;

    [Header("---------- JUMP ----------")]
    public float jumpHeight = 6.5f;
    public int numberOfJumpsAllowed = 2;
    [Range(1, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;

    [Header("---------- JUMP CUT ----------")]
    [Range(0.02f, 0.3f)] public float timeForUpwardsCancel = 0.027f;

    [Header("---------- JUMP APEX ----------")]
    [Range(0.5f, 1f)] public float apexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.075f;

    [Header("---------- JUMP BUFFER ----------")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("---------- JUMP COYOTE TIME ----------")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("---------- JUMP VISUALISATION TOOL ----------")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5, 100)] public int arcResolution = 20;
    [Range(0, 500)] public int visualisationSteps = 90;

    [Header("---------- GRAVITY ----------")]
    public float gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }

    [Header("---------- GROUNDED / COLLISION CHECKS ----------")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float headWidth = 0.75f;

    [Header("---------- DEBUG ----------")]
    public bool debugShowIsGroundedBox;
    public bool debugShowHeadBumpedBox;

    [Header("---------- Calculus ----------")]
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;
    private Rigidbody2D playerRB;

    //Movement variables
    private Vector2 moveVelocity;
    private Vector2 movement;
    private bool isFacingRight;

    //Collision check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private RaycastHit2D wallHit;
    private RaycastHit2D lastWallHit;
    public bool isGrounded { get; private set; }
    private bool bumpedHead;

    //Jump variables
    public float verticalVelocity { get; private set; }
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    //Apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    //Jump buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    //Coyote time variables
    private float coyoteTimer;




    private void Awake()
    {
        isFacingRight = true;
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement = move.action.ReadValue<Vector2>();
        JumpChecks();
        CountTimers();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if(isGrounded)
        {
            Move(groundAcceleration, groundDeceleration, movement);
        }
        else
        {
            Move(airAcceleration, airDeceleration, movement);
        }
    }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }

    #region Movements
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if(moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            // if(RUN), then same code but with run values
            targetVelocity = new Vector2(moveInput.x, 0f) * maxWalkSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            playerRB.linearVelocity = new Vector2(moveVelocity.x, playerRB.linearVelocity.y);
        }
        else if(moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            playerRB.linearVelocity = new Vector2(moveVelocity.x, playerRB.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    #endregion

    #region Jump
    private void JumpChecks()
    {
        //When we press the jump button
        if (jump.action.WasPressedThisFrame())
        {
            jumpBufferTimer = jumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }
        
        //When we release the jump button
        if (jump.action.WasReleasedThisFrame())
        {
            if(jumpBufferTimer > 0)
            {
                jumpReleasedDuringBuffer = true;
            }

            if(isJumping && verticalVelocity > 0f)
            {
                if(isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }

        //Initiate jump with jump buffering and coyote time
        if (jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }
        }

        //Double jump
        else if (jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < numberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }

        //Air jump after coyote time lapsed
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < numberOfJumpsAllowed -1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        //Landed
        if((isJumping || isFastFalling) && isGrounded  && verticalVelocity <= 0f)
        {
            Debug.Log("hey");
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int _numberOfJumpsUsed)
    {
        if(!isJumping)
        {
            isJumping = true;
        }
        jumpBufferTimer = 0f;
        numberOfJumpsUsed += _numberOfJumpsUsed;
        verticalVelocity = initialJumpVelocity;
    }

    private void Jump()
    {
        //Apply gravity while jumping
        if(isJumping)
        {
            //Check for head bump
            if (bumpedHead)
            {
                isFastFalling = true;
            }
            //Gravity on ascending
            if (verticalVelocity >= 0f)
            {
                //Apex controls
                apexPoint = Mathf.InverseLerp(initialJumpVelocity, 0f, verticalVelocity);
                if (apexPoint > apexThreshold)
                {
                    if(!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }
                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if(timePastApexThreshold < apexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                        }
                    }
                }

                //Gravity on ascending but not past apex threshold
                else
                {
                    verticalVelocity += gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }

            //Gravity on descending
            else if (!isFastFalling)
            {
                verticalVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (verticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }
        }

        //Jump cut
        if (isFastFalling)
        {
            if(fastFallTime >= timeForUpwardsCancel)
            {
                verticalVelocity += gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if(fastFallTime < timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / timeForUpwardsCancel));
            }

            fastFallTime += Time.fixedDeltaTime;
        }

        //Normal gravity while falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        //Clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);

        playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, verticalVelocity);
    }
    #endregion

    #region Collision checks
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, groundDetectionRayLength);
        
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, groundDetectionRayLength, groundLayer);
        if (groundHit.collider != null)
        {
            Debug.Log("working");
            isGrounded = true;
        }
        else { isGrounded = false; }

        #region Debug visualisation
        if (debugShowIsGroundedBox)
        {
            Color rayColor;
            if(isGrounded)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * headWidth, headDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, headDetectionRayLength, groundLayer);
        if(headHit.collider != null)
        {
            bumpedHead = true;
        }
        else { bumpedHead = false; }

        #region Debug visualisation
        if (debugShowHeadBumpedBox)
        {
            float headWidthVar = headWidth;

            Color rayColor;
            if (bumpedHead)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidthVar, boxCastOrigin.y), Vector2.up * headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidthVar, boxCastOrigin.y), Vector2.up * headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidthVar, boxCastOrigin.y + headDetectionRayLength), Vector2.right * boxCastSize.x * headWidthVar, rayColor);
        }
        #endregion
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }
    #endregion

    #region Timers
    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if(!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = jumpCoyoteTime;
        }
    }
    #endregion
}
