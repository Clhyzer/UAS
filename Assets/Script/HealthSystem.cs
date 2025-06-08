using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk Slider
using UnityEngine.Events; // Diperlukan untuk UnityEvent

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI & Effects")]
    public Slider healthSlider; // Referensi langsung ke Slider Health Bar
    public GameObject deathEffectPrefab; // Prefab efek kematian (Partikel, animasi, dll.)

    // Referensi ke HitEffectManager jika ada (opsional, untuk dipanggil secara langsung)
    // Sebaiknya gunakan OnDamageTaken event untuk memicu HitEffectManager
    // private HitEffectManager hitEffectManager; 

    // Events yang bisa dipanggil saat health berubah atau entitas mati
    [Header("Health Events")]
    public UnityEvent OnHealthChanged; // Dipanggil setiap kali currentHealth berubah
    public UnityEvent OnDamageTaken;   // Dipanggil saat entitas menerima damage (untuk efek hit!)
    public UnityEvent OnDeath;         // Dipanggil saat currentHealth <= 0

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // Pastikan UI terupdate di awal

        // Dapatkan referensi ke HitEffectManager jika ada di objek yang sama
        // hitEffectManager = GetComponent<HitEffectManager>();
    }

    // Fungsi untuk menerima damage
    public void TakeDamage(int damageAmount)
    {
        Debug.Log($"[{gameObject.name}] Menerima damage: {damageAmount}");

        if (currentHealth <= 0)
        {
            Debug.Log($"[{gameObject.name}] Sudah mati, tidak bisa menerima damage lagi.");
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Pastikan health di antara 0 dan maxHealth

        Debug.Log($"[{gameObject.name}] Mengambil {damageAmount} damage! Health saat ini: {currentHealth}/{maxHealth}");

        UpdateHealthUI(); // Update UI health bar
        OnHealthChanged?.Invoke(); // Panggil event perubahan health

        // Pemicu Efek Hit!
        OnDamageTaken?.Invoke(); // Panggil event ini untuk memicu HitEffectManager

        // Anda juga bisa memanggilnya secara langsung jika Anda memilih tidak menggunakan Unity Event untuk ini:
        // if (hitEffectManager != null)
        // {
        //     hitEffectManager.PlayHitEffect();
        // }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Fungsi untuk menyembuhkan
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Pastikan health tidak melebihi maxHealth

        Debug.Log($"[{gameObject.name}] Menyembuhkan {healAmount}. Health saat ini: {currentHealth}/{maxHealth}");
        UpdateHealthUI(); // Update UI health bar
        OnHealthChanged?.Invoke(); // Panggil event perubahan health
    }

    // Fungsi untuk memperbarui tampilan health bar UI
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
        else
        {
            Debug.LogWarning($"Health Slider not assigned to {gameObject.name}'s HealthSystem. UI will not update.", this);
        }
    }

    // Fungsi kematian
    void Die()
    {
        Debug.Log($"[{gameObject.name}] Telah mati!");

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        OnDeath?.Invoke(); // Panggil event kematian

        gameObject.SetActive(false); 
    }
}