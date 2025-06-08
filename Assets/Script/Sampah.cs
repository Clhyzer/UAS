using System.Collections.Generic;
using UnityEngine;

public class Sampah : MonoBehaviour
{
    public int maxCapacity = 5;  // Maksimum kapasitas tempat sampah
    private List<GameObject> storedItems = new List<GameObject>();  // Daftar item yang sudah dibuang
    public GameObject iconE;  // Ikon E (optional, jika diperlukan)

    // Fungsi untuk membuang item ke dalam tempat sampah
    public void BuangSampah(GameObject item)
    {
        if (storedItems.Count < maxCapacity)
        {
            storedItems.Add(item);  // Tambahkan item ke dalam tempat sampah
            Destroy(item);  // Hapus item dari scene
            Debug.Log("Item dibuang ke tempat sampah. Total item: " + storedItems.Count);
        }
        else
        {
            Debug.Log("Tempat sampah penuh!");
        }
    }

    // Menampilkan ikon saat player berada di dekat tempat sampah
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (iconE != null)
                iconE.SetActive(true);  // Tampilkan ikon E saat bisa membuang item
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (iconE != null)
                iconE.SetActive(false);  // Sembunyikan ikon E saat player keluar dari trigger
        }
    }
}
