using UnityEngine;

public class Projectile : DamageBase
{
	public float Duration = 3f;

	public GameObject Spawn;

	private float timeTemp;

	private NetworkView networkViewer;

	private bool isQuitting;

	[RPC]
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
		networkViewer = GetComponent<NetworkView>();
		timeTemp = Time.time;
	}

	private void Update()
	{
		if ((((bool)networkViewer && networkViewer.isMine) || (!Network.isServer && !Network.isClient)) && Time.time >= timeTemp + Duration)
		{
			OnDead();
		}
		if ((bool)networkViewer && networkViewer.isMine && (Network.isServer || Network.isClient))
		{
			networkViewer.RPC("PositionUpdate", RPCMode.Others, base.transform.position, base.transform.rotation, OwnerID);
		}
	}

	[RPC]
	private void OnDead()
	{
		if (Network.isServer || Network.isClient)
		{
			Network.Destroy(base.gameObject);
			if ((bool)networkViewer)
			{
				Network.RemoveRPCs(networkViewer.viewID);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
