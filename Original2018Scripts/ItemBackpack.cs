using System.Collections.Generic;
using UnityEngine;

public class ItemBackpack : ItemData
{
	public List<ItemCollector> Items = new List<ItemCollector>();

	private void Start()
	{
		if (!networkViewer || !networkViewer.isMine || (!Network.isServer && !Network.isClient))
		{
			return;
		}
		string empty = string.Empty;
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		foreach (ItemCollector item in Items)
		{
			if (item.Item != null)
			{
				text = text + item.Item.ItemID + ",";
				text2 = text2 + item.Num + ",";
				text3 = text3 + item.NumTag + ",";
			}
		}
		empty = text + "|" + text2 + "|" + text3;
		Debug.Log("Copy " + empty);
		networkViewer.RPC("GetItem", RPCMode.Others, empty);
	}

	[RPC]
	private void GetItem(string itemdatatext)
	{
		ItemManager itemManager = (ItemManager)Object.FindObjectOfType(typeof(ItemManager));
		if (!itemManager)
		{
			return;
		}
		Debug.Log("Get itemlist : " + itemdatatext);
		string[] array = itemdatatext.Split("|"[0]);
		string[] array2 = array[0].Split(","[0]);
		string[] array3 = array[1].Split(","[0]);
		string[] array4 = array[2].Split(","[0]);
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i] != string.Empty)
			{
				ItemCollector itemCollector = new ItemCollector();
				itemCollector.Item = itemManager.GetItemDataByID(array2[i]);
				int.TryParse(array3[i], out itemCollector.Num);
				int.TryParse(array4[i], out itemCollector.NumTag);
				itemCollector.Active = true;
				AddItem(itemCollector);
			}
		}
	}

	public void AddItem(ItemCollector item)
	{
		if (item.Active)
		{
			Items.Add(item);
		}
	}

	public override void Pickup(CharacterSystem character)
	{
		if (!(character != null) || !character.inventory || Items == null)
		{
			return;
		}
		foreach (ItemCollector item in Items)
		{
			if (item.Item != null)
			{
				Debug.Log(string.Concat("Pick up ", item.Item, "Num tag ", item.NumTag));
				character.inventory.AddItemByItemData(item.Item, item.Num, item.NumTag, -1);
			}
		}
		RemoveItem();
	}
}
