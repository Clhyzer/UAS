using System.Collections;
using UnityEngine;

public class HitEffectManager : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    public float hitDuration = 0.2f;

    [Header("Particles")]
    public GameObject hitParticlesPrefab;
    public Vector3 particleOffset;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioSource audioSource;

    private Color originalColor;
    private Coroutine hitEffectCoroutine;

    void Start()
    {
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer belum di-assign di HitEffectManager.");
        }

        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayHitEffect()
    {
        Debug.Log("PlayHitEffect DIPANGGIL");

        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
            spriteRenderer.color = originalColor;
        }

        hitEffectCoroutine = StartCoroutine(ChangeColorTemporarily());

        if (hitParticlesPrefab != null)
        {
            GameObject p = Instantiate(hitParticlesPrefab, transform.position + particleOffset, Quaternion.identity);
            Destroy(p, 2f);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private IEnumerator ChangeColorTemporarily()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(hitDuration);
            spriteRenderer.color = originalColor;
        }
    }
}
