using UnityEngine;

public class Projectile : DamageBase
{
	public float Duration = 3f;

	public GameObject Spawn;

	private float timeTemp;

	private bool isQuitting;

	private void PositionUpdate(Vector3 position, Quaternion rotation, string id)
	{
		OwnerID = id;
		base.transform.position = Vector3.Lerp(base.transform.position, position, 1f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, 1f);
	}

	private void OnApplicationQuit()
	{
		isQuitting = true;
	}

	private void OnDestroy()
	{
		if (!isQuitting && (bool)Spawn && !Application.isLoadingLevel)
		{
			GameObject gameObject = Object.Instantiate(Spawn, base.transform.position, base.transform.rotation);
			DamageBase component = gameObject.GetComponent<DamageBase>();
			if ((bool)component)
			{
				component.OwnerID = OwnerID;
				component.OwnerTeam = OwnerTeam;
			}
		}
	}

	private void Start()
	{
		timeTemp = Time.time;
	}

	private void Update()
	{
		if (Time.time >= timeTemp + Duration)
		{
			OnDead();
		}
		{
			PositionUpdate(base.transform.position, base.transform.rotation, OwnerID);
		}
	}

	private void OnDead()
	{
	
		{
			Object.Destroy(base.gameObject);
		}
	}
}
