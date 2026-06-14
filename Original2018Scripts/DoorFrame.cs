using UnityEngine;

public class DoorFrame : MonoBehaviour
{
	public Animator animator;

	public bool IsOpen;

	private float timeTemp;

	public float Cooldown = 0.5f;

	public string DoorKey = string.Empty;

	private NetworkView networkViewer;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
	}

	public void Access(CharacterSystem character)
	{
		if (!Network.isClient && !Network.isServer)
		{
			AccessDoor(DoorKey);
			return;
		}
		if (Network.isClient && (bool)networkViewer)
		{
			networkViewer.RPC("accessDoor", RPCMode.Server, DoorKey);
		}
		if (Network.isServer)
		{
			AccessDoor(DoorKey);
		}
	}

	private void AccessDoor(string key)
	{
		if (key == DoorKey && Time.time > timeTemp + Cooldown)
		{
			IsOpen = !IsOpen;
			timeTemp = Time.time;
		}
	}

	private void Update()
	{
		if ((bool)animator)
		{
			animator.SetBool("IsOpen", IsOpen);
		}
		if ((bool)networkViewer && Network.isServer)
		{
			int num = 0;
			if (IsOpen)
			{
				num = 1;
			}
			networkViewer.RPC("doorUpdate", RPCMode.Others, num);
		}
	}

	[RPC]
	private void accessDoor(string key)
	{
		AccessDoor(key);
	}

	[RPC]
	private void doorUpdate(int doorstate)
	{
		switch (doorstate)
		{
		case 0:
			IsOpen = false;
			break;
		case 1:
			IsOpen = true;
			break;
		}
	}
}
