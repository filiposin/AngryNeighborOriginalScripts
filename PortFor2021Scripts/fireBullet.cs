using UnityEngine;

public class fireBullet : MonoBehaviour
{
	public Rigidbody bulletPrefab;

	[Tooltip("location to launch bullet from.")]
	public Transform fireSpot;

	[Tooltip("only select the Enemy Layer.")]
	public LayerMask enemyLayers;

	private void Start()
	{
		if (!fireSpot)
		{
			fireSpot = base.transform;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && (bool)bulletPrefab)
		{
			Rigidbody rigidbody = Object.Instantiate(bulletPrefab, fireSpot.position, fireSpot.rotation);
			rigidbody.AddForce(fireSpot.forward * 2000f);
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			alertEnemies(base.transform.position, 1000f);
		}
	}

	public void alertEnemies(Vector3 center, float radius)
	{
		Collider[] array = Physics.OverlapSphere(center, radius, enemyLayers);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessageUpwards("wakeUp", 1f, SendMessageOptions.DontRequireReceiver);
		}
	}
}
