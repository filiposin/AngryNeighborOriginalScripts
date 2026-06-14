using UnityEngine;
using UnityEngine.EventSystems;

public class DropSwapItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public GUIItemCollector GUIItem;

	public void Start()
	{
		if (GUIItem == null)
		{
			GUIItem = GetComponent<GUIItemCollector>();
		}
	}

	public void OnDrop(PointerEventData data)
	{
		if (GUIItem == null || GUIItem.Item == null || !GUIItem.currentInventory)
		{
			return;
		}
		GUIItemCollector dropSprite = GetDropSprite(data);
		if (!(dropSprite != null) || dropSprite.Item == null)
		{
			return;
		}
		ItemCollector item = new ItemCollector();
		GUIItem.currentInventory.CopyCollector(item, GUIItem.Item);
		if ((!(GUIItem.Type == "Stock") && !(GUIItem.Type == "Inventory")) || !(dropSprite.Type != "Shop"))
		{
			return;
		}
		{
			if (GUIItem.currentInventory != dropSprite.currentInventory)
			{
				if ((bool)GUIItem.currentInventory.character && GUIItem.currentInventory.character.IsMine)
				{
					GUIItem.currentInventory.PutCollector(dropSprite.Item, GUIItem.Item.InventoryIndex);
					dropSprite.currentInventory.PutCollectorSync(item, dropSprite.Item.InventoryIndex);
				}
				else
				{
					GUIItem.currentInventory.PutCollectorSync(dropSprite.Item, GUIItem.Item.InventoryIndex);
					dropSprite.currentInventory.PutCollector(item, dropSprite.Item.InventoryIndex);
				}
			}
			else if ((bool)GUIItem.currentInventory.character && GUIItem.currentInventory.character.IsMine)
			{
				GUIItem.currentInventory.SwarpCollector(GUIItem.Item, dropSprite.Item);
			}
			else
			{
				GUIItem.currentInventory.PutCollectorSync(dropSprite.Item, GUIItem.Item.InventoryIndex);
				dropSprite.currentInventory.PutCollectorSync(item, dropSprite.Item.InventoryIndex);
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
