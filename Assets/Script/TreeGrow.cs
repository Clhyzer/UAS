using System.Collections;
using UnityEngine;

public class TreeGrow : MonoBehaviour
{
    public GameObject[] growthStages;  // Prefab tiap tahap pertumbuhan
    public float growTime = 5f;        // Waktu antar tahap

    private int currentStage = 0;

    // Fungsi untuk memulai pertumbuhan pohon
    public void StartGrowing()
    {
        StartCoroutine(GrowTree());
    }

    IEnumerator GrowTree()
    {
        while (currentStage < growthStages.Length)
        {
            // Menampilkan tahap pertumbuhan saat ini
            for (int i = 0; i < growthStages.Length; i++)
                growthStages[i].SetActive(i == currentStage);

            currentStage++;  // Pindah ke tahap berikutnya
            yield return new WaitForSeconds(growTime);  // Tunggu selama waktu pertumbuhan
        }
    }
}
