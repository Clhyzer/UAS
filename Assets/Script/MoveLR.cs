using UnityEngine;

public class MoveLR : MonoBehaviour
{
    public float speed = 3f;
    public float moveRange = 5f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveRange)
        {
            direction *= -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ketika player menyentuh kayu
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Tempel player ke kayu
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ketika player meninggalkan kayu
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Lepaskan parent
        }
    }
}
