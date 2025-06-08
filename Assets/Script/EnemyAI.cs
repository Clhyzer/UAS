using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1f;
    public int damageToPlayer = 10;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    private Vector2 initialPosition;
    private float lastAttackTime;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isNearEdge;

    void Start()
    {
        initialPosition = transform.position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > attackRange)
            {
                CheckEdge(); // ← deteksi jurang

                if (!isNearEdge)
                {
                    MoveTowardsX(player.position);
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    animator.SetBool("isMoving", false);
                }
            }
            else
            {
                animator.SetBool("isMoving", false);
                Attack();
            }
        }
        else
        {
            float distanceToStart = Vector2.Distance(transform.position, initialPosition);
            if (distanceToStart > 0.1f)
            {
                CheckEdge();

                if (!isNearEdge)
                {
                    MoveTowardsX(initialPosition);
                    animator.SetBool("isMoving", true);
                }
                else
                {
                    animator.SetBool("isMoving", false);
                }
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void MoveTowardsX(Vector2 target)
    {
        Vector2 targetX = new Vector2(target.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetX, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);

        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("attackTrigger");
            lastAttackTime = Time.time;

            if (Vector2.Distance(transform.position, player.position) <= attackRange + 0.5f)
            {
                PlayerHealt playerHealt = player.GetComponent<PlayerHealt>();
                if (playerHealt != null)
                {
                    playerHealt.TakeDamage(damageToPlayer);
                }
            }
        }
    }

    void CheckEdge()
    {
        Vector2 checkPosition = groundCheck.position;
        isNearEdge = !Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);

        // Debug visual
        Debug.DrawRay(checkPosition, Vector2.down * groundCheckDistance, isNearEdge ? Color.red : Color.green);
    }
}