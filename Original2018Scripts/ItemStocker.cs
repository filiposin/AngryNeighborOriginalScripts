using UnityEngine;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(CharacterInventory))]
public class ItemStocker : ObjectTrigger
{
	public string StockID = "mybox";

	public CharacterInventory inventory;

	private int updateTemp;

	private NetworkView networkViewer;

	private bool stockLoaded;

	private ObjectPlacing placing;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		networkViewer = GetComponent<NetworkView>();
		inventory = GetComponent<CharacterInventory>();
		placing = GetComponent<ObjectPlacing>();
		if ((bool)placing)
		{
			StockID = placing.ItemUID;
		}
	}

	public void OpenStock()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer))
		{
			LoadStock();
		}
		GetSyncStock();
	}

	public override void OnExit()
	{
		UnitZ.Hud.CloseSecondInventory();
		base.OnExit();
	}

	public void SaveItem(ItemCollector item)
	{
		inventory.AddItemByCollector(item);
	}

	private void Update()
	{
		if (!(inventory == null))
		{
			if (updateTemp != inventory.UpdateCount && stockLoaded)
			{
				SyncStock();
				SaveStock();
				updateTemp = inventory.UpdateCount;
			}
			UpdateFunction();
		}
	}

	public override void Pickup(CharacterSystem character)
	{
		if ((bool)character && character.IsMine)
		{
			OpenStock();
			UnitZ.Hud.OpenSecondInventory(inventory, "Stock");
		}
		base.Pickup(character);
	}

	private void SaveStock()
	{
		if (!(inventory == null))
		{
			string itemDataText = inventory.GetItemDataText();
			PlayerPrefs.SetString(StockID, itemDataText);
		}
	}

	private void LoadStock()
	{
		if (!(inventory == null))
		{
			if (PlayerPrefs.HasKey(StockID))
			{
				inventory.SetItemsFromText(PlayerPrefs.GetString(StockID));
				stockLoaded = true;
			}
			else
			{
				stockLoaded = true;
				SaveStock();
			}
		}
	}

	private void SyncStock()
	{
		if (Network.isServer && (bool)networkViewer)
		{
			string itemDataText = inventory.GetItemDataText();
			networkViewer.RPC("getStockData", RPCMode.Others, itemDataText);
		}
	}

	[RPC]
	private void getStockData(string text)
	{
		if (!(inventory == null))
		{
			inventory.SetItemsFromText(text);
		}
	}

	public void GetSyncStock()
	{
		if ((bool)networkViewer && Network.isClient)
		{
			networkViewer.RPC("getSyncStock", RPCMode.Server, null);
		}
	}

	[RPC]
	public void getSyncStock(NetworkMessageInfo messageInfo)
	{
		if (!stockLoaded)
		{
			LoadStock();
		}
		if ((bool)networkViewer && (bool)inventory)
		{
			string itemDataText = inventory.GetItemDataText();
			networkViewer.RPC("getStockData", messageInfo.sender, itemDataText);
		}
	}
}
