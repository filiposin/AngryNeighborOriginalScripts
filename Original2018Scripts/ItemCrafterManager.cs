using UnityEngine;

public class ItemCrafterManager : MonoBehaviour
{
	public ItemCrafter[] ItemCraftList;

	[HideInInspector]
	public ItemCrafter ItemSelected;

	private CharacterInventory characterInventory;

	[HideInInspector]
	public bool crafting;

	private float timeTemp;

	[HideInInspector]
	public float CraftingDuration;

	[HideInInspector]
	public float CraftingDurationNormalize;

	private void Start()
	{
		crafting = false;
		CraftingDurationNormalize = 0f;
	}

	private void Update()
	{
		if (crafting && ItemSelected != null && characterInventory != null)
		{
			bool flag = true;
			CraftingDuration = timeTemp + ItemSelected.CraftTime - Time.time;
			CraftingDurationNormalize = 1f / ItemSelected.CraftTime * CraftingDuration;
			for (int i = 0; i < ItemSelected.ItemNeeds.Length; i++)
			{
				if ((bool)ItemSelected.ItemNeeds[i].Item && characterInventory.GetItemNum(ItemSelected.ItemNeeds[i].Item) < ItemSelected.ItemNeeds[i].Num)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				Debug.Log("stop crafting");
				CancelCraft();
			}
			else if (Time.time >= timeTemp + ItemSelected.CraftTime)
			{
				CraftComplete();
			}
		}
		if (crafting && ItemSelected == null)
		{
			CancelCraft();
		}
	}

	public void CraftSelected(ItemCrafter item)
	{
		ItemSelected = item;
	}

	public bool Craft(CharacterInventory inventory)
	{
		if (ItemSelected == null || inventory == null)
		{
			return false;
		}
		characterInventory = inventory;
		for (int i = 0; i < ItemSelected.ItemNeeds.Length; i++)
		{
			if ((bool)ItemSelected.ItemNeeds[i].Item && characterInventory.GetItemNum(ItemSelected.ItemNeeds[i].Item) < ItemSelected.ItemNeeds[i].Num)
			{
				return false;
			}
		}
		crafting = true;
		timeTemp = Time.time;
		return true;
	}

	public bool CheckNeeds(ItemCrafter Crafter, CharacterInventory inventory)
	{
		if (Crafter == null || inventory == null)
		{
			return false;
		}
		for (int i = 0; i < Crafter.ItemNeeds.Length; i++)
		{
			if ((bool)Crafter.ItemNeeds[i].Item && inventory.GetItemNum(Crafter.ItemNeeds[i].Item) < Crafter.ItemNeeds[i].Num)
			{
				return false;
			}
		}
		return true;
	}

	public void CraftComplete()
	{
		if (characterInventory != null && ItemSelected != null)
		{
			for (int i = 0; i < ItemSelected.ItemNeeds.Length; i++)
			{
				if ((bool)ItemSelected.ItemNeeds[i].Item)
				{
					characterInventory.RemoveItem(ItemSelected.ItemNeeds[i].Item, ItemSelected.ItemNeeds[i].Num);
				}
			}
			characterInventory.AddItemByItemData(ItemSelected.ItemResult, ItemSelected.NumResult, -1, -1);
		}
		Debug.Log("craft complete");
		CancelCraft();
	}

	public void CancelCraft()
	{
		CraftingDurationNormalize = 0f;
		crafting = false;
		ItemSelected = null;
	}
}
