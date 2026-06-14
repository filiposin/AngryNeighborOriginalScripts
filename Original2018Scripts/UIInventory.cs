using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
	[HideInInspector]
	public CharacterInventory inventory;

	[HideInInspector]
	public ItemCrafterManager crafterManager;

	public GUISkin Skin;

	private int page;

	private Vector2 scrollPosition;

	public Vector2 size = new Vector2(450f, 300f);

	private void Start()
	{
		page = 0;
		inventory = GetComponent<CharacterInventory>();
		crafterManager = (ItemCrafterManager)Object.FindObjectOfType(typeof(ItemCrafterManager));
		StyleManager styleManager = (StyleManager)Object.FindObjectOfType(typeof(StyleManager));
		if (!Skin && (bool)styleManager)
		{
			Skin = styleManager.GetSkin(0);
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		size = new Vector2(450f, 350f);
		if ((bool)Skin)
		{
			GUI.skin = Skin;
		}
		if (((Network.isServer || Network.isClient) && !GetComponent<NetworkView>().isMine) || !(inventory != null) || !inventory.Toggle)
		{
			return;
		}
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.skin.label.fontSize = 35;
		if (GUI.Button(new Rect((float)(Screen.width / 2) - size.x / 2f - 10f, (float)(Screen.height / 2) - size.y / 2f - 50f, 100f, 35f), "Items"))
		{
			page = 0;
		}
		if (GUI.Button(new Rect((float)(Screen.width / 2) - size.x / 2f + 95f, (float)(Screen.height / 2) - size.y / 2f - 50f, 100f, 35f), "Craft") && (bool)crafterManager)
		{
			if (crafterManager.crafting)
			{
				page = 2;
			}
			else
			{
				page = 1;
			}
		}
		GUI.Box(new Rect((float)(Screen.width / 2) - size.x / 2f - 10f, (float)(Screen.height / 2) - size.y / 2f - 10f, size.x + 20f, size.y + 20f), string.Empty);
		GUI.BeginGroup(new Rect((float)(Screen.width / 2) - size.x / 2f, (float)(Screen.height / 2) - size.y / 2f, size.x, size.y), string.Empty);
		GUI.skin.label.fontSize = 19;
		int num = 0;
		switch (page)
		{
		case 0:
		{
			List<ItemCollector> items = inventory.Items;
			for (int k = 0; k < items.Count; k++)
			{
				if (items[k].Index != -1 && items[k].Num > 0)
				{
					num++;
				}
			}
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, size.x, size.y), scrollPosition, new Rect(0f, 0f, size.x - 20f, num * 35));
			for (int l = 0; l < items.Count; l++)
			{
				if (items[l].Item != null && items[l].Index != -1 && items[l].Num > 0)
				{
					GUI.Label(new Rect(20f, l * 35, 200f, 30f), items[l].Item.ItemName + " <color=lime>x " + items[l].Num + "</color>");
					if (GUI.Button(new Rect(340f, l * 35, 50f, 30f), "Use"))
					{
						inventory.EquipItemByCollector(items[l]);
					}
					if (GUI.Button(new Rect(400f, l * 35, 30f, 30f), "X"))
					{
						Debug.Log(string.Concat(items[l], "  ", items[l].Item));
						inventory.DropItemByCollector(items[l], items[l].Item.Quantity);
					}
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 1:
		{
			if (!crafterManager)
			{
				break;
			}
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, size.x, size.y), scrollPosition, new Rect(0f, 0f, size.x - 20f, crafterManager.ItemCraftList.Length * 35));
			for (int j = 0; j < crafterManager.ItemCraftList.Length; j++)
			{
				if ((bool)crafterManager.ItemCraftList[j].ItemResult)
				{
					GUI.Label(new Rect(20f, j * 35, 200f, 30f), crafterManager.ItemCraftList[j].ItemResult.ItemName);
					if (GUI.Button(new Rect(340f, j * 35, 100f, 30f), "Craft"))
					{
						crafterManager.CraftSelected(crafterManager.ItemCraftList[j]);
						page = 2;
					}
				}
			}
			GUI.EndScrollView();
			break;
		}
		case 2:
		{
			if (!(crafterManager != null) || crafterManager.ItemSelected == null)
			{
				break;
			}
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, size.x, size.y - 60f), scrollPosition, new Rect(0f, 0f, size.x - 20f, crafterManager.ItemSelected.ItemNeeds.Length * 35));
			bool flag = true;
			for (int i = 0; i < crafterManager.ItemSelected.ItemNeeds.Length; i++)
			{
				if ((bool)crafterManager.ItemSelected.ItemNeeds[i].Item)
				{
					int itemNum = inventory.GetItemNum(crafterManager.ItemSelected.ItemNeeds[i].Item);
					int num2 = crafterManager.ItemSelected.ItemNeeds[i].Num;
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.Label(new Rect(20f, i * 35, 200f, 30f), crafterManager.ItemSelected.ItemNeeds[i].Item.ItemName + "  <color=silver>X " + num2 + "</color>");
					GUI.skin.label.alignment = TextAnchor.MiddleRight;
					if (itemNum >= crafterManager.ItemSelected.ItemNeeds[i].Num)
					{
						GUI.Label(new Rect(340f, i * 35, 100f, 30f), "<color=lime>Ready</color>");
						continue;
					}
					flag = false;
					int num3 = num2 - itemNum;
					GUI.Label(new Rect(340f, i * 35, 100f, 30f), "<color=red>Need " + num3 + "</color>");
				}
			}
			GUI.EndScrollView();
			if (crafterManager.crafting)
			{
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect(10f, size.y - 50f, size.x - 110f, 40f), "Crafting.. " + Mathf.Floor(crafterManager.CraftingDuration) + " s.");
				if (GUI.Button(new Rect(size.x - 110f, size.y - 50f, 100f, 40f), "Cancel"))
				{
					crafterManager.CancelCraft();
				}
			}
			else if (GUI.Button(new Rect(10f, size.y - 50f, size.x - 20f, 40f), "Craft") && flag)
			{
				crafterManager.Craft(inventory);
			}
			break;
		}
		}
		GUI.EndGroup();
	}
}
