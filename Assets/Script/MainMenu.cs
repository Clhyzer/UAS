using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Fungsi untuk pindah ke Level 1
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1"); // Pastikan nama scene cocok
    }

    // Fungsi untuk keluar dari game
    public void ExitGame()
    {
        Debug.Log("Game exited");
        Application.Quit(); // Tidak akan berfungsi di editor
    }
}
