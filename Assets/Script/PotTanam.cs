using UnityEngine;

public class PotTanam : MonoBehaviour
{
    public Transform tanamPoint;
    private bool sudahDitanam = false;

    public bool BisaTanam()
    {
        return !sudahDitanam;
    }

    public void TanamPohon()
    {
        sudahDitanam = true;
        Debug.Log("Pohon ditanam di pot!");
    }

    public Vector3 GetTanamPoint()
    {
        return tanamPoint != null ? tanamPoint.position : transform.position;
    }
}
