using UnityEngine;
using System.Collections; // For Coroutines

public class BossAI : MonoBehaviour
{
    // --- Boss Settings ---
    [Header("Boss Settings")]
    public float moveSpeed = 3f;        // Speed when patrolling/general movement
    public float chaseSpeed = 5f;       // Speed when chasing the player
    public float attackRange = 2.0f;    // Distance to trigger attack
    public float detectionRange = 10f;  // Distance to detect player
    public float attackCooldown = 1.5f; // Time between attacks
    public int attackDamage = 20;       // Damage dealt per attack
    public float returnSpeed = 4f;

    [Header("Boss Attack Pattern")]
    public float retreatSpeed = 4f;     // Speed when retreating
    public float retreatDistance = 5f;  // How far the boss retreats after attacking
    public float advanceDelay = 2f;     // Time to wait before advancing again

    // --- Chase Boundaries (Optional, for restricting boss area) ---
    [Header("Chase Boundaries")]
    public bool useChaseBoundaries = false; // Toggle to enable/disable boundaries
    public float leftChaseEndpoint;     // Leftmost X-coordinate the boss can chase to
    public float rightChaseEndpoint;    // Rightmost X-coordinate the boss can chase to

    // --- References ---
    [Header("References")]
    public Transform playerTransform;   // Reference to the Player's Transform
    public Animator animator;           // Reference to the Animator component
    public Rigidbody2D rb;              // Reference to the Rigidbody2D component

    [Header("Audio")]
    public AudioSource audioSource;     // AudioSource component
    public AudioClip attackSound;       // Attack sound clip
    public AudioClip footstepSound;     // Footstep sound clip

    // --- Internal State ---
    private Vector2 initialPosition;    // Boss's starting position
    private BossState currentState;     // Current state of the boss
    private bool isFacingRight = true;
    private bool canAttack = true;
    private Vector2 retreatPoint;       // Target position for retreating

    // To prevent footstep overlap
    private bool isFootstepPlaying = false;

    // Enum for Boss States
    public enum BossState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Advance,
        ReturnToStart
    }

    void Start()
    {
        initialPosition = transform.position;
        currentState = BossState.Idle;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player object with tag 'Player' not found. Assign playerTransform manually.");
            }
        }
    }

    void Update()
    {
        HandleAnimations();

        switch (currentState)
        {
            case BossState.Idle:
                IdleState();
                break;
            case BossState.Patrol:
                PatrolState();
                break;
            case BossState.Chase:
                ChaseState();
                break;
            case BossState.Attack:
                AttackState();
                break;
            case BossState.Retreat:
                RetreatState();
                break;
            case BossState.Advance:
                AdvanceState();
                break;
            case BossState.ReturnToStart:
                ReturnToStartState();
                break;
        }

        HandleFootstepSound();
    }

    void IdleState()
    {
        if (IsPlayerInDetectionRange())
        {
            currentState = BossState.Chase;
        }
    }

    void PatrolState()
    {
        if (IsPlayerInDetectionRange())
        {
            currentState = BossState.Chase;
            return;
        }

        if (isFacingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x > initialPosition.x + detectionRange)
                Flip();
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x < initialPosition.x - detectionRange)
                Flip();
        }
    }

    void ChaseState()
    {
        if (playerTransform == null)
        {
            currentState = BossState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance((Vector2)transform.position, (Vector2)playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            currentState = BossState.Idle;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = BossState.Attack;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (useChaseBoundaries && (transform.position.x < leftChaseEndpoint || transform.position.x > rightChaseEndpoint))
        {
            currentState = BossState.ReturnToStart;
            return;
        }

        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();
    }

    void AttackState()
    {
        if (playerTransform == null)
        {
            currentState = BossState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance((Vector2)transform.position, (Vector2)playerTransform.position);

        if (distanceToPlayer > attackRange)
        {
            currentState = BossState.Chase;
            return;
        }

        if (canAttack)
            StartCoroutine(PerformAttack());
    }

    void RetreatState()
    {
        if (Vector2.Distance((Vector2)transform.position, retreatPoint) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(WaitForAdvance());
            return;
        }

        Vector2 direction = (retreatPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * retreatSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();
    }

    void AdvanceState()
    {
        if (playerTransform == null)
        {
            currentState = BossState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance((Vector2)transform.position, (Vector2)playerTransform.position);

        if (distanceToPlayer <= attackRange * 1.2f)
        {
            currentState = BossState.Chase;
            return;
        }

        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();
    }

    void ReturnToStartState()
    {
        if (Vector2.Distance((Vector2)transform.position, initialPosition) < 0.1f)
        {
            transform.position = initialPosition;
            currentState = BossState.Idle;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 directionToStart = (initialPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(directionToStart.x * returnSpeed, rb.linearVelocity.y);

        if (directionToStart.x > 0 && !isFacingRight)
            Flip();
        else if (directionToStart.x < 0 && isFacingRight)
            Flip();
    }

    bool IsPlayerInDetectionRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance((Vector2)transform.position, (Vector2)playerTransform.position) <= detectionRange;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");

        // Play attack sound
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        PlayerHealt playerHealt = playerTransform.GetComponent<PlayerHealt>();
        if (playerHealt != null)
        {
            playerHealt.TakeDamage(attackDamage);
            Debug.Log("Boss attacked Player for " + attackDamage + " damage!");
        }
        else
        {
            Debug.LogWarning("PlayerHealt component not found on player!");
        }

        float attackAnimationDuration = 0.5f;
        yield return new WaitForSeconds(attackAnimationDuration);

        float retreatDirectionX = (transform.position.x > playerTransform.position.x) ? 1f : -1f;
        retreatPoint = (Vector2)transform.position + new Vector2(retreatDirectionX * retreatDistance, 0f);

        currentState = BossState.Retreat;
        yield return new WaitForSeconds(Mathf.Max(0, attackCooldown - attackAnimationDuration));
        canAttack = true;
    }

    IEnumerator WaitForAdvance()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(advanceDelay);
        currentState = BossState.Advance;
    }

    void HandleAnimations()
    {
        animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);
        animator.SetBool("IsAttacking", currentState == BossState.Attack);
    }

    void HandleFootstepSound()
    {
        // Play footstep sound only when moving (walking or chasing) and on ground
        if ((currentState == BossState.Chase || currentState == BossState.Patrol || currentState == BossState.Advance || currentState == BossState.ReturnToStart)
            && rb.linearVelocity.magnitude > 0.1f)
        {
            if (!isFootstepPlaying && footstepSound != null && audioSource != null)
            {
                audioSource.clip = footstepSound;
                audioSource.loop = true;
                audioSource.Play();
                isFootstepPlaying = true;
            }
        }
        else
        {
            if (isFootstepPlaying && audioSource != null)
            {
                audioSource.Stop();
                isFootstepPlaying = false;
            }
        }
    }

    // For visual debugging in Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (useChaseBoundaries)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(leftChaseEndpoint, transform.position.y - 2f, 0), new Vector3(leftChaseEndpoint, transform.position.y + 2f, 0));
            Gizmos.DrawLine(new Vector3(rightChaseEndpoint, transform.position.y - 2f, 0), new Vector3(rightChaseEndpoint, transform.position.y + 2f, 0));
        }
    }
}
