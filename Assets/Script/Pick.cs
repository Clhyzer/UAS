using System.Collections;
using System.Collections.Generic; // Pastikan ini ada untuk List
using UnityEngine;

public class Pick : MonoBehaviour
{
    public Transform holdSpot; // Posisi di mana item akan dipegang
    public LayerMask pickUpMask; // Layer untuk item yang bisa diambil
    public LayerMask tanahMask; // Layer untuk tanah tempat menanam
    public LayerMask tempatSampahMask; // Layer untuk tempat sampah
    public LayerMask potMask; // Layer untuk pot tanaman

    public string surfaceTag = "Meja"; // Tag untuk permukaan tempat menaruh objek (misal: "Meja")

    public Vector3 Direction { get; set; } // Arah pandang pemain (harus diset dari script player movement)

    // --- PERUBAHAN UTAMA DI SINI ---
    private List<GameObject> itemsHolding = new List<GameObject>(); // Menggunakan List untuk menyimpan banyak item
    public int maxHoldItems = 3; // Batas maksimum item yang bisa dipegang

    void Start()
    {
        // Pastikan list diinisialisasi
        if (itemsHolding == null)
        {
            itemsHolding = new List<GameObject>();
        }
    }

    void Update()
    {
        // Ambil item (Tombol E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Cek apakah jumlah item yang dipegang belum mencapai batas maksimum
            if (itemsHolding.Count < maxHoldItems)
            {
                // OverlapCircle untuk mendeteksi item di sekitar
                Collider2D pickUpItemCollider = Physics2D.OverlapCircle(transform.position + Direction, 0.4f, pickUpMask);
                if (pickUpItemCollider)
                {
                    GameObject pickedItem = pickUpItemCollider.gameObject;
                    
                    // Pastikan item yang diambil belum dipegang
                    if (!itemsHolding.Contains(pickedItem))
                    {
                        // Set posisi item ke holdSpot (akan menumpuk jika banyak)
                        // Anda mungkin perlu menyesuaikan posisi Y agar terlihat menumpuk
                        Vector3 itemPosition = holdSpot.position;
                        // Contoh: Menumpuk item secara vertikal
                        itemPosition.y += itemsHolding.Count * 0.2f; // Sesuaikan offset 0.2f sesuai ukuran item

                        pickedItem.transform.position = itemPosition;
                        pickedItem.transform.parent = holdSpot; // Set parent ke holdSpot, bukan langsung ke player

                        // Nonaktifkan simulasi Rigidbody2D agar item tidak jatuh
                        if (pickedItem.TryGetComponent<Rigidbody2D>(out var rb))
                            rb.simulated = false;

                        // Tambahkan item ke dalam list
                        itemsHolding.Add(pickedItem);
                        Debug.Log($"Mengambil item: {pickedItem.name}. Total item: {itemsHolding.Count}");
                    }
                }
            }
            else
            {
                Debug.Log($"Tidak bisa mengambil item lagi. Batas maksimum ({maxHoldItems}) tercapai.");
            }
        }

        // Buang item ke tempat sampah (Tombol Q)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (itemsHolding.Count > 0) // Pastikan ada item yang dipegang
            {
                Collider2D dekatSampah = Physics2D.OverlapCircle(transform.position + Direction, 0.4f, tempatSampahMask);

                // Mengambil item paling terakhir yang diambil untuk dibuang
                GameObject itemToDispose = itemsHolding[itemsHolding.Count - 1]; 

                if (dekatSampah && SampahManager.Instance != null)
                {
                    if (itemToDispose.CompareTag("Sampah"))
                    {
                        SampahManager.Instance.BuangSampah(itemToDispose);
                        itemsHolding.RemoveAt(itemsHolding.Count - 1); // Hapus dari list
                        Debug.Log($"Membuang sampah: {itemToDispose.name}. Sisa item: {itemsHolding.Count}");

                        // Sesuaikan posisi item yang tersisa jika ada
                        UpdateHeldItemPositions();
                    }
                    else
                    {
                        Debug.Log("Item yang dipegang bukan sampah.");
                    }
                }
                else
                {
                    Debug.Log("Kamu tidak berada di dekat tempat sampah.");
                }
            }
            else
            {
                Debug.Log("Tidak ada item yang dipegang untuk dibuang.");
            }
        }

        // Tanam di tanah atau pot (Tombol R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (itemsHolding.Count > 0) // Pastikan ada item yang dipegang
            {
                GameObject itemToPlant = itemsHolding[itemsHolding.Count - 1]; // Ambil item paling terakhir

                if (itemToPlant.CompareTag("Bibit"))
                {
                    bool planted = false;

                    // Tanam di tanah
                    Collider2D tanah = Physics2D.OverlapCircle(transform.position + Direction, 0.4f, tanahMask);
                    if (tanah && tanah.TryGetComponent<TanahTanam>(out var tanahScript))
                    {
                        if (tanahScript.BisaTanam())
                        {
                            ReleaseItemFromHold(itemToPlant); // Lepaskan item dari genggaman
                            itemToPlant.transform.position = tanahScript.GetTanamPoint();
                            itemToPlant.layer = LayerMask.NameToLayer("Untagged"); // Ubah layer jika perlu
                            tanahScript.TanamPohon();
                            itemsHolding.Remove(itemToPlant);
                            planted = true;
                            Debug.Log($"Menanam bibit: {itemToPlant.name} di tanah. Sisa item: {itemsHolding.Count}");
                            UpdateHeldItemPositions();
                        }
                    }

                    if (!planted)
                    {
                        // Tanam di pot
                        Collider2D pot = Physics2D.OverlapCircle(transform.position + Direction, 0.4f, potMask);
                        if (pot && pot.TryGetComponent<PotTanam>(out var potScript))
                        {
                            if (potScript.BisaTanam())
                            {
                                ReleaseItemFromHold(itemToPlant); // Lepaskan item dari genggaman
                                itemToPlant.transform.position = potScript.GetTanamPoint();
                                itemToPlant.layer = LayerMask.NameToLayer("Untagged"); // Ubah layer jika perlu
                                potScript.TanamPohon();
                                itemsHolding.Remove(itemToPlant);
                                planted = true;
                                Debug.Log($"Menanam bibit: {itemToPlant.name} di pot. Sisa item: {itemsHolding.Count}");
                                UpdateHeldItemPositions();
                            }
                        }
                    }

                    if (!planted)
                    {
                        Debug.Log("Tidak menemukan tanah atau pot untuk menanam.");
                    }
                }
                else
                {
                    Debug.Log("Item yang dipegang bukan bibit.");
                }
            }
            else
            {
                Debug.Log("Tidak ada item yang dipegang untuk ditanam.");
            }
        }

        // Letakkan objek di permukaan (berdasarkan tag) dengan tombol T
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (itemsHolding.Count > 0) // Pastikan ada item yang dipegang
            {
                GameObject itemToPlace = itemsHolding[itemsHolding.Count - 1]; // Ambil item paling terakhir

                Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position + Direction, 0.4f);
                bool placed = false;
                foreach (var col in nearby)
                {
                    if (col.CompareTag(surfaceTag))
                    {
                        ReleaseItemFromHold(itemToPlace); // Lepaskan item dari genggaman
                        // Letakkan item di atas permukaan, sesuaikan offset Y jika perlu
                        itemToPlace.transform.position = col.transform.position + Vector3.up * 0.5f; 
                        
                        itemsHolding.Remove(itemToPlace);
                        placed = true;
                        Debug.Log($"Meletakkan item: {itemToPlace.name} di atas permukaan. Sisa item: {itemsHolding.Count}");
                        UpdateHeldItemPositions();
                        break; // Keluar dari loop setelah menemukan permukaan
                    }
                }

                if (!placed)
                {
                    Debug.Log("Tidak ada permukaan (tag '" + surfaceTag + "') untuk meletakkan objek.");
                }
            }
            else
            {
                Debug.Log("Tidak ada item yang dipegang untuk diletakkan.");
            }
        }
    }

    // Fungsi helper untuk melepaskan item dari genggaman (parenting dan Rigidbody)
    private void ReleaseItemFromHold(GameObject item)
    {
        item.transform.parent = null; // Lepaskan dari parent (holdSpot)
        if (item.TryGetComponent<Rigidbody2D>(out var rb))
            rb.simulated = true; // Aktifkan kembali simulasi fisika
    }

    // Fungsi untuk memperbarui posisi visual item yang sedang dipegang
    private void UpdateHeldItemPositions()
    {
        for (int i = 0; i < itemsHolding.Count; i++)
        {
            GameObject item = itemsHolding[i];
            Vector3 itemPosition = holdSpot.position;
            itemPosition.y += i * 0.2f; // Sesuaikan offset ini agar item menumpuk dengan rapi
            item.transform.position = itemPosition;
        }
    }

    // Fungsi ini bisa digunakan oleh script lain (misal: PlayerMovement)
    // untuk memberi tahu arah pandang player
    public void SetDirection(Vector3 newDirection)
    {
        Direction = newDirection.normalized; // Normalisasi untuk memastikan vektor arah unit
    }

    // Visualisasi OverlapCircle di Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Pastikan Direction terinisialisasi sebelum digunakan di Gizmos
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(transform.position + Direction, 0.4f);
        }
        else
        {
            // Saat di editor dan game tidak berjalan, gunakan Vector3.right sebagai default Direction
            Gizmos.DrawWireSphere(transform.position + Vector3.right * 0.5f, 0.4f); 
        }
    }
}