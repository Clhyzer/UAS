using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
     [SerializeField] private string nextSceneName; // Nama scene tujuan

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
