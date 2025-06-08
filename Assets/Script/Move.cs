using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float Jumping = 10f;
    public float moveSpeed = 5f;
    private float Horizontal;
    private bool isFacingRight = true;

    public Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Ground & Wall Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkRadius = 0.2f;

    public bool canMove = true;
    public PhysicsMaterial2D noFrictionMaterial;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;
    private bool isGrounded;
    private bool isTouchingWall;

    [Header("Footstep Audio")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public float footstepInterval = 0.4f;
    private float footstepTimer = 0f;

    void Start()
    {
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sharedMaterial = noFrictionMaterial;
        }
    }

    void Update()
    {
        if (!canMove)
        {
            Horizontal = 0f;
            UpdateAnimator(0f);
            return;
        }

        Horizontal = Input.GetAxisRaw("Horizontal");

        isGrounded = CheckIsGrounded();
        isTouchingWall = CheckIsTouchingWall();

        // Lompat
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Jumping);

            if (Horizontal != 0)
            {
                rb.linearVelocity += new Vector2(Horizontal * 0.5f, 0f);
            }
        }

        // Potong lompatan agar lebih responsif
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        UpdateAnimator(rb.linearVelocity.x);
        Putar();
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        // Gerakan horizontal
        rb.linearVelocity = new Vector2(Horizontal * moveSpeed, rb.linearVelocity.y);

        // Wall slide logic
        if (!isGrounded && isTouchingWall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
    }

    private void UpdateAnimator(float horizontalSpeed)
    {
        if (animator != null)
        {
            animator.SetBool("Jumping", !isGrounded);
            animator.SetFloat("x", Mathf.Abs(horizontalSpeed));
            animator.SetFloat("y", rb.linearVelocity.y);
        }
    }

    private void Putar()
    {
        if ((isFacingRight && Horizontal < 0f) || (!isFacingRight && Horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool CheckIsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private bool CheckIsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
    }

    private void HandleFootsteps()
{
    if (Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded)
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (footstepSource != null && footstepClip != null && !footstepSource.isPlaying)
            {
                footstepSource.clip = footstepClip;
                footstepSource.Play();
            }
            footstepTimer = footstepInterval;
        }
    }
    else
    {
        // Stop footstep sound when not moving or not grounded
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
        footstepTimer = 0f;
    }
}

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }
}
