using UnityEngine;

public class PlantSlotChecker : MonoBehaviour
{
    public Transform[] slots; // Titik lokasi tanah untuk menanam bibit (5 slot)
    public float checkRadius = 0.5f; // Radius deteksi bibit di setiap slot
    public LayerMask bibitLayer; // Layer tempat bibit berada
    public GameObject objectToActivate; // Objek yang akan diaktifkan jika semua slot terisi

    private bool activated = false;

    void Update()
    {
        if (activated) return;

        if (AreAllSlotsFilled())
        {
            activated = true;
            Debug.Log("Semua slot sudah terisi bibit!");
            if (objectToActivate != null)
                objectToActivate.SetActive(true);
        }
    }

    bool AreAllSlotsFilled()
    {
        foreach (Transform slot in slots)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(slot.position, checkRadius, bibitLayer);

            bool foundBibit = false;
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Bibit"))
                {
                    foundBibit = true;
                    break;
                }
            }

            if (!foundBibit)
                return false; // Jika salah satu slot belum ada bibit
        }

        return true; // Semua slot terisi
    }

    void OnDrawGizmosSelected()
    {
        if (slots == null) return;

        Gizmos.color = Color.green;
        foreach (Transform slot in slots)
        {
            if (slot != null)
                Gizmos.DrawWireSphere(slot.position, checkRadius);
        }
    }
}
