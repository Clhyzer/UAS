using UnityEngine;
using System.Collections.Generic;
using TMPro; // Tambahkan untuk TextMeshPro

public class SampahManager : MonoBehaviour
{
    public static SampahManager Instance;  // Singleton pattern
    public int collectedSampah = 0;
    public int requiredSampah = 5;
    public string questID = "buang_sampah";
    public NPC npc;  // NPC pemberi quest
    public GameObject iconQ;  // Ikon Q (opsional)
    public TextMeshProUGUI sampahCounterText;  // UI counter sampah

    private List<GameObject> storedItems = new List<GameObject>();
    private bool playerInRange = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateSampahUI(); // Inisialisasi tampilan counter
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            GameObject itemToDispose = FindObjectToDispose();
            if (itemToDispose != null)
            {
                BuangSampah(itemToDispose);
            }
        }
    }

    private GameObject FindObjectToDispose()
    {
        // Placeholder: sesuaikan dengan sistem inventory kamu
        return null;
    }

    public void BuangSampah(GameObject item)
    {
        if (storedItems.Count < requiredSampah)
        {
            storedItems.Add(item);
            collectedSampah++;
            Destroy(item);
            Debug.Log("Sampah yang dikumpulkan: " + collectedSampah);

            UpdateSampahUI();

            if (collectedSampah >= requiredSampah)
            {
                CompleteQuest();
            }
        }
        else
        {
            Debug.Log("Tempat sampah penuh!");
        }
    }

    private void UpdateSampahUI()
    {
        if (sampahCounterText != null)
        {
            sampahCounterText.text = collectedSampah + "/" + requiredSampah;
        }
    }

    public void CompleteQuest()
    {
        Debug.Log("Quest selesai! Sampah sudah terkumpul.");
        QuestManager.Instance.CompleteQuest(questID);

        if (npc != null)
        {
            npc.OnQuestComplete();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (iconQ != null)
                iconQ.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (iconQ != null)
                iconQ.SetActive(false);
        }
    }
}
