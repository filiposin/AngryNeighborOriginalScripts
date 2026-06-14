using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public LevelPreset[] LevelPresets;

	private DayNightCycle dayNight;

	private TreesManager Trees;

	private NetworkView networkViewer;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		Trees = (TreesManager)Object.FindObjectOfType(typeof(TreesManager));
	}

	private void Update()
	{
		if (dayNight == null)
		{
			dayNight = (DayNightCycle)Object.FindObjectOfType(typeof(DayNightCycle));
		}
		if (Network.isServer && (bool)dayNight && (bool)networkViewer)
		{
			networkViewer.RPC("dayTimeUpdate", RPCMode.Others, dayNight.Timer);
		}
	}

	[RPC]
	private void dayTimeUpdate(float time)
	{
		if ((bool)dayNight)
		{
			dayNight.Timer = time;
		}
	}

	public void ResetAllTrees()
	{
		if (Network.isServer && (bool)networkViewer)
		{
			networkViewer.RPC("resetAllTrees", RPCMode.All, null);
		}
	}

	public void GetInitializeeScene()
	{
		if (Network.isClient && (bool)networkViewer)
		{
			networkViewer.RPC("UpdateRemovedTrees", RPCMode.Server, null);
		}
	}

	public void SendRemovedTreeIndex(int index)
	{
		if (Network.isServer && (bool)networkViewer)
		{
			networkViewer.RPC("sendRemovedTreeIndex", RPCMode.Others, index);
		}
	}

	[RPC]
	private void resetAllTrees()
	{
		if (Trees == null)
		{
			Trees = (TreesManager)Object.FindObjectOfType(typeof(TreesManager));
		}
		if (!(Trees == null))
		{
			Trees.ResetTrees();
		}
	}

	[RPC]
	private void sendRemovedTreeIndex(int index)
	{
		if (Trees == null)
		{
			Trees = (TreesManager)Object.FindObjectOfType(typeof(TreesManager));
		}
		if (!(Trees == null))
		{
			Trees.RemoveATrees(index);
		}
	}

	[RPC]
	public void UpdateRemovedTrees()
	{
		if (Trees == null)
		{
			Trees = (TreesManager)Object.FindObjectOfType(typeof(TreesManager));
		}
		if (!(Trees == null) && Network.isServer && (bool)networkViewer)
		{
			networkViewer.RPC("getRemovedIndex", RPCMode.Others, Trees.GetRemovedTrees());
		}
	}

	[RPC]
	private void getRemovedIndex(string indexremoved)
	{
		if (Trees == null)
		{
			Trees = (TreesManager)Object.FindObjectOfType(typeof(TreesManager));
		}
		if (!(Trees == null))
		{
			Trees.UpdateRemovedTrees(indexremoved);
		}
	}
}
