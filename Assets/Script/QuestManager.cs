using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> activeQuests = new HashSet<string>();  // Daftar quest yang sedang aktif

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Menambahkan quest ke daftar aktif
    public void AddQuest(string questID)
    {
        if (!activeQuests.Contains(questID))
        {
            activeQuests.Add(questID);  // Menambahkan quest ke dalam daftar aktif
            Debug.Log("Quest Diterima: " + questID);
        }
    }

    // Menyelesaikan quest dan menghapusnya dari daftar aktif
    public void CompleteQuest(string questID)
    {
        if (activeQuests.Contains(questID))
        {
            activeQuests.Remove(questID);  // Menghapus quest yang sudah selesai
            Debug.Log("Quest Selesai: " + questID);
        }
    }

    // Memeriksa apakah quest sedang aktif
    public bool HasQuest(string questID)
    {
        return activeQuests.Contains(questID);
    }
}
