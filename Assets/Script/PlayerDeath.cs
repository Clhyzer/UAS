using UnityEngine;
using UnityEngine.SceneManagement; // Untuk Load Scene
using UnityEngine.UI; // Untuk UI Button

public class PlayerDeath : MonoBehaviour
{
    public GameObject deathUI; // UI yang muncul setelah player mati
    public Button restartButton; // Tombol restart
    public Button mainMenuButton; // Tombol main menu

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika player masuk ke collider yang menyebabkan kematian
        if (other.CompareTag("Zone"))  // Ganti tag sesuai objek yang menyebabkan kematian
        {
            Die();
        }
    }

    public void Die()
    {
        // Nonaktifkan kontrol player (atau bisa matikan movement)
        Time.timeScale = 0f;  // Menghentikan waktu, bisa matikan player sementara

        // Menampilkan UI pilihan setelah mati
        deathUI.SetActive(true);

        // Menambahkan fungsi untuk tombol
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void RestartGame()
    {
        // Reload scene saat ini untuk restart
        Time.timeScale = 1f;  // Mengaktifkan kembali waktu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Memuat ulang scene yang sedang berjalan
    }

    void GoToMainMenu()
    {
        // Mengarah ke Main Menu
        Time.timeScale = 1f;  // Mengaktifkan kembali waktu
        SceneManager.LoadScene("Main Menu");  // Ganti dengan nama scene MainMenu
    }
}
