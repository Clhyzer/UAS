using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1f;
    public LayerMask targetLayer;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;

    [Header("Audio Settings")]
    public AudioClip attackSound;
    [Tooltip("AudioSource khusus untuk suara attack (jika tidak diisi, akan dibuat otomatis)")]
    public AudioSource attackAudioSource;

    private Animator anim;
    private float nextAttackTime = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator tidak ditemukan di " + gameObject.name);
        }

        // Siapkan atau buat AudioSource
        if (attackAudioSource == null)
        {
            attackAudioSource = GetComponent<AudioSource>();
            if (attackAudioSource == null)
            {
                attackAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Konfigurasi AudioSource
        attackAudioSource.playOnAwake = false;
        attackAudioSource.loop = false;
        attackAudioSource.spatialBlend = 0f;
        attackAudioSource.volume = 1f;
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetButtonDown("Fire1"))
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        Debug.Log("Menyerang pada waktu: " + Time.time);

        // Trigger animasi
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // Putar suara attack
        if (attackSound != null && attackAudioSource != null)
        {
            attackAudioSource.Stop(); // Opsional, untuk hentikan suara sebelumnya jika overlap
            attackAudioSource.PlayOneShot(attackSound);
        }
        else
        {
            Debug.LogWarning("attackSound atau AudioSource belum di-assign!");
        }

        // Beri damage
        DamageTargets();
    }

    public void DamageTargets()
    {
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);

        foreach (Collider2D target in hitTargets)
        {
            HealthSystem targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} menyerang {target.name} dengan {attackDamage} damage");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
