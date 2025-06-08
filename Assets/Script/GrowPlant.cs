using UnityEngine;
using System.Collections;

public class GrowPlant : MonoBehaviour
{
    public float growDuration = 2f;
    private Vector3 targetScale = Vector3.one;
    private Vector3 initialScale = Vector3.zero;

    private void Start()
    {
        transform.localScale = initialScale;
        StartCoroutine(Grow());
    }

    public void StartGrowing()
    {
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / growDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
