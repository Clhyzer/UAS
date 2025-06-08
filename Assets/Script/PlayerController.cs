using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    [Header("Audio Settings (Footsteps)")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float horizontalInput;
    private bool isFacingRight = true;
    public bool canMove = true;
    public PhysicsMaterial2D noFrictionMaterial;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb == null) Debug.LogError("Rigidbody2D not found!", this);
        if (anim == null) Debug.LogWarning("Animator not found!", this);
        if (groundCheck == null) Debug.LogWarning("GroundCheck not assigned!", this);
        if (footstepSource == null) Debug.LogWarning("Footstep AudioSource not assigned!", this);

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            if (noFrictionMaterial != null)
            {
                rb.sharedMaterial = noFrictionMaterial;
            }
        }

        // Setup AudioSource
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            footstepSource.clip = footstepClip;
        }
    }

    void Update()
    {
        if (!canMove)
        {
            horizontalInput = 0f;
            UpdateAnimator(0f, rb.linearVelocity.y);
            StopFootstepSound();
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckIsGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim?.SetTrigger("Jump");

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                rb.linearVelocity += new Vector2(horizontalInput * 0.5f, 0f);
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        HandleFlip();
        UpdateAnimator(rb.linearVelocity.x, rb.linearVelocity.y);
        HandleFootsteps();
    }

    void FixedUpdate()
    {
        if (!canMove) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateAnimator(float horizontalSpeed, float verticalSpeed)
    {
        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetFloat("Speed", Mathf.Abs(horizontalSpeed));
        }
    }

    private void HandleFlip()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool CheckIsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleFootsteps()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        if (isGrounded && speed > 0.1f)
        {
            if (footstepSource != null && !footstepSource.isPlaying && footstepClip != null)
            {
                footstepSource.Play();
            }
        }
        else
        {
            StopFootstepSound();
        }
    }

    private void StopFootstepSound()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

    // Fungsi untuk trigger attack, contoh panggil ini saat player menyerang
    public void Attack()
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
        // Tambahkan logika serangan di sini jika perlu
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
