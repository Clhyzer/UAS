using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk Slider

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider; // Referensi ke komponen Slider
    public HealthSystem targetHealthSystem; // Referensi ke HealthSystem dari objek yang diikutinya
    public Vector3 offset = new Vector3(0, 1f, 0); // Offset posisi health bar dari target

    void Start()
    {
        // Pastikan kita memiliki Slider dan HealthSystem yang valid
        if (healthSlider == null)
        {
            Debug.LogError("Health Slider not assigned to HealthBarUI!", this);
            enabled = false; // Nonaktifkan script jika tidak ada slider
            return;
        }

        if (targetHealthSystem == null)
        {
            // Coba temukan HealthSystem di parent atau objek yang sama
            targetHealthSystem = GetComponentInParent<HealthSystem>();
            if (targetHealthSystem == null)
            {
                Debug.LogError("Target HealthSystem not assigned or found in parent for HealthBarUI!", this);
                enabled = false; // Nonaktifkan script jika tidak ada HealthSystem
                return;
            }
        }

        // Setup awal slider
        healthSlider.maxValue = targetHealthSystem.maxHealth;
        UpdateHealthBar(); // Panggil update awal

        // Langganan ke event OnHealthChanged dari HealthSystem
        targetHealthSystem.OnHealthChanged.AddListener(UpdateHealthBar);
        targetHealthSystem.OnDeath.AddListener(OnTargetDeath); // Optional: sembunyikan bar saat mati
    }

    void OnDestroy()
    {
        // Pastikan untuk berhenti melanggan saat objek hancur untuk menghindari error
        if (targetHealthSystem != null)
        {
            targetHealthSystem.OnHealthChanged.RemoveListener(UpdateHealthBar);
            targetHealthSystem.OnDeath.RemoveListener(OnTargetDeath);
        }
    }

    void Update()
    {
        // Posisi health bar mengikuti posisi target
        if (targetHealthSystem != null)
        {
            transform.position = targetHealthSystem.transform.position + offset;
            // Pastikan bar selalu menghadap kamera (Billboard effect)
            transform.LookAt(Camera.main.transform.position);
            transform.forward = -transform.forward; // Flip agar tidak terbalik
        }
    }

    // Fungsi untuk mengupdate nilai slider
    void UpdateHealthBar()
    {
        if (healthSlider != null && targetHealthSystem != null)
        {
            healthSlider.value = targetHealthSystem.currentHealth;
        }
    }

    // Fungsi yang dipanggil saat target mati
    void OnTargetDeath()
    {
        // Menyembunyikan health bar saat target mati
        gameObject.SetActive(false);
    }
}