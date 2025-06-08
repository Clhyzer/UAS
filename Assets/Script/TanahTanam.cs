using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanahTanam : MonoBehaviour
{
    public Transform titikTanam;           // Titik posisi di mana objek akan ditaruh
    public GameObject pohonPrefab;         // Prefab pohon yang akan tumbuh
    private bool sudahDitanam = false;     // Cek apakah sudah ditanam

    public bool BisaTanam()
    {
        return !sudahDitanam;
    }

    public void TanamPohon()
    {
        if (!sudahDitanam && pohonPrefab != null && titikTanam != null)
        {
            Instantiate(pohonPrefab, titikTanam.position, Quaternion.identity);
            sudahDitanam = true;
        }
    }

    public Vector3 GetTanamPoint()
    {
        return titikTanam != null ? titikTanam.position : transform.position;
    }
}
