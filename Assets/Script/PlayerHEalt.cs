using UnityEngine;
using UnityEngine.UI;

public class PlayerHealt : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public GameObject deathEffect;

    [Header("Hit Effects")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float hitDuration = 0.2f;
    public GameObject hitParticlesPrefab;
    public AudioClip hitSound;
    public Vector3 particleOffset;
    public AudioSource audioSource;

    private Color originalColor;
    private bool isHitEffectActive = false;

    private PlayerDeath playerDeath;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        originalColor = spriteRenderer.color;
        playerDeath = GetComponent<PlayerDeath>();

        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        PlayHitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Player mati karena health 0");

        if (playerDeath != null)
        {
            playerDeath.Die();
        }
    }

    void PlayHitEffect()
    {
        if (spriteRenderer != null && !isHitEffectActive)
        {
            StartCoroutine(HitFlash());
        }

        if (hitParticlesPrefab != null)
        {
            GameObject particles = Instantiate(hitParticlesPrefab, transform.position + particleOffset, Quaternion.identity);
            Destroy(particles, 2f);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    System.Collections.IEnumerator HitFlash()
    {
        isHitEffectActive = true;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitDuration);
        spriteRenderer.color = originalColor;
        isHitEffectActive = false;
    }
}
