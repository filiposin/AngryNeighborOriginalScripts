using UnityEngine;

public class bullet : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag != "enemy")
		{
			Object.Destroy(base.gameObject);
		}
		if (collision.collider.tag == "Enemy" || collision.collider.tag == "head")
		{
			collision.collider.SendMessageUpwards("Damage", 1f);
		}
	}
}
