using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public GameObject carriedItem = null;  // Item yang dibawa oleh player
    private GameObject nearbyItem = null;  // Item yang terdekat dengan player
    public GameObject iconE;               // Ikon "E" untuk interaksi
    public Transform handPosition;         // Posisi tangan player, drag pada inspector
    public bool HasItem;

    void Start()
    {
        iconE.SetActive(false); // Pastikan ikon E tersembunyi di awal
    }

    void Update()
    {
        // Menampilkan ikon E jika ada item yang bisa diambil
        if (nearbyItem != null && carriedItem == null)
        {
            iconE.SetActive(true); // Menampilkan ikon E saat dekat item yang bisa diambil
        }
        else
        {
            iconE.SetActive(false); // Menyembunyikan ikon E saat tidak ada item yang bisa diambil
        }

        // Menangkap item dengan menekan tombol E
        if (Input.GetKeyDown(KeyCode.E) && carriedItem == null && nearbyItem != null)
        {
            carriedItem = nearbyItem;  // Menyimpan referensi item yang diambil
            carriedItem.SetActive(true); // Menampilkan item setelah diambil

            // Pindahkan item ke posisi tangan player
            carriedItem.transform.position = handPosition.position; // Menyusun item ke tangan player
            carriedItem.transform.SetParent(handPosition); // Menjadikan tangan player sebagai parent item

            // Menghancurkan item yang ada di tanah (langsung hilang)
            nearbyItem.SetActive(false); // Item yang ada di tanah sekarang disembunyikan

            Debug.Log("Item diambil: " + carriedItem.name);
            nearbyItem = null;  // Menghapus referensi item yang diambil
            iconE.SetActive(false);  // Menyembunyikan ikon E setelah item diambil
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Memastikan hanya item yang memiliki tag "Item" yang bisa ditangkap
        if (other.CompareTag("Item"))
        {
            nearbyItem = other.gameObject; // Menyimpan item yang terdekat dengan player
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Jika player keluar dari area item, hilangkan referensi item
        if (other.CompareTag("Item") && other.gameObject == nearbyItem)
        {
            nearbyItem = null;
            iconE.SetActive(false); // Menyembunyikan ikon E saat tidak ada item di sekitar
        }
    }
}
