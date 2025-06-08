using UnityEngine;

public class PlantMission : MonoBehaviour
{
    [Header("Slot Tanah (Posisi Bibit Harus Tertanam)")]
    public Transform[] slots; // Posisi tanah yang akan dicek apakah ada bibit
    public float checkRadius = 0.5f; // Radius pengecekan collider

    [Header("Pengaturan Layer Bibit")]
    public LayerMask bibitLayer; // Layer tempat bibit berada

    [Header("Quest")]
    public string questID; // ID quest yang harus aktif agar misi ini berlaku

    [Header("Objek yang Diaktifkan Setelah Misi")]
    public GameObject missionCompleteObject; // Objek yang diaktifkan saat misi selesai, contoh: pintu

    private bool missionCompleted = false;

    void Update()
    {
        if (missionCompleted) return;

        // Periksa apakah quest dengan ID sesuai aktif
        if (QuestManager.Instance != null && QuestManager.Instance.HasQuest(questID))
        {
            if (AreAllSlotsFilled())
            {
                missionCompleted = true;
                CompleteMission();
            }
        }
    }

    bool AreAllSlotsFilled()
    {
        int filledCount = 0;

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
            {
                Debug.Log("Slot belum berisi bibit di: " + slot.name);
                return false;
            }

            filledCount++;
        }

        Debug.Log("Semua slot terisi bibit: " + filledCount + "/" + slots.Length);
        return true;
    }

    void CompleteMission()
    {
        Debug.Log("✅ Misi penanaman selesai!");

        // Tandai misi selesai di quest manager
        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest(questID);

        // Aktifkan objek misalnya pintu, notifikasi, dll
        if (missionCompleteObject != null)
        {
            missionCompleteObject.SetActive(true);
            Debug.Log("🎉 Objek misi diaktifkan: " + missionCompleteObject.name);
        }
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
