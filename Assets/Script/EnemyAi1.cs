using UnityEngine;
using System.Collections;

public class EnemyAi1 : MonoBehaviour
{
    // --- Pengaturan Musuh ---
    [Header("Enemy Settings")]
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float attackRange = 1.5f;
    public float detectionRange = 7f;
    public float patrolRange = 5f;
    public float returnSpeed = 4f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    // --- Referensi Health System ---
    [Header("Health")]
    public HealthSystem healthSystem; // << BARU: Referensi ke HealthSystem

    // --- End Points Pengejaran ---
    [Header("Chase Boundaries")]
    public float leftChaseEndpoint;
    public float rightChaseEndpoint;

    // --- Referensi ---
    [Header("References")]
    public Transform playerTransform;
    public Animator animator;
    public Rigidbody2D rb;

    // --- Internal State ---
    private Vector2 initialPosition;
    private EnemyState currentState;
    private bool isFacingRight = true;
    private bool canAttack = true;

    // Enum untuk State Musuh
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        ReturnToStart
    }

    void Start()
    {
        initialPosition = transform.position;
        currentState = EnemyState.Idle;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        // << BARU: Dapatkan referensi HealthSystem dari objek ini
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                Debug.LogError("HealthSystem component not found on " + gameObject.name + "! Enemy needs a HealthSystem.");
                enabled = false; // Nonaktifkan script jika tidak ada health system
                return;
            }
        }

        // Contoh: Set playerTransform jika belum diatur di Inspector
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player object with tag 'Player' not found. Please assign playerTransform manually or ensure the player has the correct tag.");
            }
        }

        // << BARU: Daftarkan fungsi kematian musuh ke event OnDeath dari HealthSystem
        healthSystem.OnDeath.AddListener(OnEnemyDeath);
    }

    void OnDestroy()
    {
        // << BARU: Pastikan untuk melepas listener saat objek dihancurkan untuk mencegah error
        if (healthSystem != null)
        {
            healthSystem.OnDeath.RemoveListener(OnEnemyDeath);
        }
    }


    void Update()
    {
        // << BARU: Jangan lakukan AI jika musuh sudah mati
        if (healthSystem.currentHealth <= 0) return;

        HandleAnimations();

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Patrol:
                PatrolState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
            case EnemyState.ReturnToStart:
                ReturnToStartState();
                break;
        }
    }

    void IdleState()
    {
        if (IsPlayerInDetectionRange())
        {
            currentState = EnemyState.Chase;
        }
    }

    void PatrolState()
    {
        if (IsPlayerInDetectionRange())
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (isFacingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x > initialPosition.x + patrolRange)
            {
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x < initialPosition.x - patrolRange)
            {
                Flip();
            }
        }
    }

    void ChaseState()
    {
        if (playerTransform == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (transform.position.x < leftChaseEndpoint || transform.position.x > rightChaseEndpoint)
        {
            currentState = EnemyState.ReturnToStart;
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void AttackState()
    {
        if (playerTransform == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    void ReturnToStartState()
    {
        if (Vector2.Distance(transform.position, initialPosition) < 0.1f)
        {
            transform.position = initialPosition;
            currentState = EnemyState.Idle;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 directionToStart = (initialPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(directionToStart.x * returnSpeed, rb.linearVelocity.y);

        if (directionToStart.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (directionToStart.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    // --- Fungsi Helper ---

    bool IsPlayerInDetectionRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= detectionRange;
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

        PlayerHealt playerHealt = playerTransform.GetComponent<PlayerHealt>();
        if (playerHealt != null)
        {
            playerHealt.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked Player for " + attackDamage + " damage!");
        } else {
            Debug.LogWarning("PlayerHealt component not found on player object!");
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void HandleAnimations()
    {
        animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);

        if (currentState == EnemyState.Attack && canAttack == false)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    // << BARU: Fungsi yang dipanggil saat musuh mati (dari HealthSystem)
    void OnEnemyDeath()
    {
        Debug.Log(gameObject.name + " is dead. Initiating death sequence.");
        // Nonaktifkan Rigidbody agar tidak terpengaruh fisika lagi
        if (rb != null) rb.simulated = false;
        // Nonaktifkan collider agar tidak bisa berinteraksi lagi
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Pemicu animasi kematian jika ada
        animator.SetTrigger("Die"); 

        // Nonaktifkan script AI agar tidak jalan lagi
        this.enabled = false;

        // Contoh: Hancurkan objek setelah beberapa detik
        Destroy(gameObject, 3f); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(leftChaseEndpoint, transform.position.y, transform.position.z), new Vector3(rightChaseEndpoint, transform.position.y, transform.position.z));
        Gizmos.DrawWireSphere(new Vector3(leftChaseEndpoint, transform.position.y, transform.position.z), 0.2f);
        Gizmos.DrawWireSphere(new Vector3(rightChaseEndpoint, transform.position.y, transform.position.z), 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(initialPosition, 0.5f);
    }
}