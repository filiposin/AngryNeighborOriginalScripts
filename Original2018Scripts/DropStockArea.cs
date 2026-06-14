using UnityEngine;
using UnityEngine.EventSystems;

public class DropStockArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[HideInInspector]
	public CharacterInventory inventory;

	public GUIItemLoader loader;

	public bool IsMine;

	public string Type;

	private GUIItemCollector itemDrop;

	public ItemData Currency;

	public void Start()
	{
	}

	public void Trade(int number)
	{
		if (loader == null || itemDrop == null || !(itemDrop != null) || !(inventory != null) || itemDrop.Item == null)
		{
			return;
		}
		if (number <= 0)
		{
			number = 1;
		}
		if (!(inventory != itemDrop.currentInventory))
		{
			return;
		}
		if (!Network.isServer && !Network.isClient)
		{
			if ((bool)itemDrop.currentInventory.character && itemDrop.currentInventory.character.IsMine)
			{
				switch (Type)
				{
				case "Stock":
					Debug.Log("Move to Stock");
					if (number > itemDrop.Item.Num)
					{
						number = itemDrop.Item.Num;
					}
					if (inventory.AddItemByCollector(itemDrop.Item, number, -1))
					{
						itemDrop.currentInventory.RemoveItemByCollector(itemDrop.Item, number);
					}
					else
					{
						Debug.Log("Stock is full");
					}
					break;
				case "Shop":
					Debug.Log("Sell to shop");
					if (number > itemDrop.Item.Num)
					{
						number = itemDrop.Item.Num;
					}
					itemDrop.currentInventory.RemoveItemByCollector(itemDrop.Item, number);
					if ((bool)Currency)
					{
						itemDrop.currentInventory.AddItemByItemData(Currency, itemDrop.Item.Item.Price * number, -1, -1);
					}
					break;
				}
				return;
			}
			switch (itemDrop.Type)
			{
			case "Stock":
				Debug.Log("Move to my inventory.");
				if (number > itemDrop.Item.Num)
				{
					number = itemDrop.Item.Num;
				}
				if (inventory.AddItemByCollector(itemDrop.Item, number, -1))
				{
					itemDrop.currentInventory.RemoveItemByCollector(itemDrop.Item, number);
				}
				else
				{
					Debug.Log("Inventory Is full");
				}
				break;
			case "Shop":
				Debug.Log("Buy to my inventory");
				if (inventory.AddItemTest(itemDrop.Item, number))
				{
					if ((Currency != null && inventory.RemoveItem(Currency, itemDrop.Item.Item.Price)) || itemDrop.Item.Item.Price <= 0)
					{
						inventory.AddItemByCollector(itemDrop.Item, number, -1);
					}
				}
				else
				{
					Debug.Log("Inventory is full");
				}
				break;
			}
			return;
		}
		if ((bool)itemDrop.currentInventory.character && itemDrop.currentInventory.character.IsMine)
		{
			switch (Type)
			{
			case "Stock":
				Debug.Log("Move to Stock");
				if (number > itemDrop.Item.Num)
				{
					number = itemDrop.Item.Num;
				}
				inventory.AddItemByCollectorSync(itemDrop.Item, number, -1);
				itemDrop.currentInventory.RemoveItemByCollectorSync(itemDrop.Item, number);
				break;
			case "Shop":
				Debug.Log("Sell to shop");
				if (number > itemDrop.Item.Num)
				{
					number = itemDrop.Item.Num;
				}
				itemDrop.currentInventory.AddItemByItemData(Currency, itemDrop.Item.Item.Price * number, -1, -1);
				inventory.AddItemByCollectorSync(itemDrop.Item, number, -1);
				itemDrop.currentInventory.RemoveItemByCollectorSync(itemDrop.Item, number);
				break;
			}
			return;
		}
		switch (itemDrop.Type)
		{
		case "Stock":
			Debug.Log("Move to my inventory.");
			if (number > itemDrop.Item.Num)
			{
				number = itemDrop.Item.Num;
			}
			if (inventory.AddItemTest(itemDrop.Item, number))
			{
				inventory.AddItemByCollectorSync(itemDrop.Item, number, -1);
				itemDrop.currentInventory.RemoveItemByCollectorSync(itemDrop.Item, number);
			}
			else
			{
				Debug.Log("Inventory is full");
			}
			break;
		case "Shop":
			Debug.Log("Buy to my inventory");
			if (inventory.AddItemTest(itemDrop.Item, number))
			{
				if ((Currency != null && inventory.RemoveItem(Currency, itemDrop.Item.Item.Price * number)) || itemDrop.Item.Item.Price <= 0)
				{
					inventory.AddItemByCollectorSync(itemDrop.Item, number, -1);
				}
			}
			else
			{
				Debug.Log("Inventory is full");
			}
			break;
		}
	}

	public void OnDrop(PointerEventData data)
	{
		itemDrop = GetDropSprite(data);
		if (!(loader == null) && !(itemDrop == null))
		{
			inventory = loader.currentInventory;
			if (inventory != itemDrop.currentInventory && TooltipTrade.Instance != null)
			{
				TooltipTrade.Instance.StartTrade(itemDrop.Item, this);
			}
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
	}

	public void OnPointerExit(PointerEventData data)
	{
	}

	private GUIItemCollector GetDropSprite(PointerEventData data)
	{
		GameObject pointerDrag = data.pointerDrag;
		if (pointerDrag == null)
		{
			return null;
		}
		if ((bool)pointerDrag.GetComponent<GUIItemCollector>())
		{
			return pointerDrag.GetComponent<GUIItemCollector>();
		}
		return null;
	}
}
