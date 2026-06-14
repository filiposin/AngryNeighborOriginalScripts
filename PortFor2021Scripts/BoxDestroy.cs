using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Okno"))
        {
            Destroy(collision.gameObject);
        }
    }
}