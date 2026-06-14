using UnityEngine;
using UnityEngine.UI;

public class GUICrafting : GUICraft
{
	public ValueBar Process;

	public Button CraftButton;

	private void Start()
	{
		if ((bool)Icon)
		{
			Icon.enabled = false;
		}
		if ((bool)Name)
		{
			Name.enabled = false;
		}
		if ((bool)Process)
		{
			Process.gameObject.SetActive(false);
		}
		if ((bool)CraftButton)
		{
			CraftButton.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (!UnitZ.itemCraftManager)
		{
			return;
		}
		if ((bool)Process)
		{
			if (UnitZ.itemCraftManager.crafting && UnitZ.itemCraftManager.ItemSelected == Crafter)
			{
				Process.gameObject.SetActive(true);
				Process.ValueMax = 1f;
				Process.Value = 1f - UnitZ.itemCraftManager.CraftingDurationNormalize;
				Process.CustomText = Mathf.Floor(UnitZ.itemCraftManager.CraftingDuration) + " SEC.";
			}
			else
			{
				Process.gameObject.SetActive(false);
			}
		}
		if ((bool)CraftButton)
		{
			if (UnitZ.itemCraftManager.crafting)
			{
				CraftButton.gameObject.SetActive(false);
			}
			else if ((bool)UnitZ.playerManager && (bool)UnitZ.playerManager.playingCharacter)
			{
				if (UnitZ.itemCraftManager.CheckNeeds(Crafter, UnitZ.playerManager.playingCharacter.inventory))
				{
					CraftButton.gameObject.SetActive(true);
				}
				else
				{
					CraftButton.gameObject.SetActive(false);
				}
			}
		}
		if (Crafter != null && Crafter.ItemResult != null)
		{
			if (Icon != null && Crafter.ItemResult.ImageSprite != null)
			{
				Icon.sprite = Crafter.ItemResult.ImageSprite;
				Icon.enabled = true;
			}
			if (Name != null)
			{
				Name.text = Crafter.ItemResult.ItemName;
				Name.enabled = true;
			}
		}
	}

	public void CancelCraft()
	{
		if ((bool)UnitZ.itemCraftManager && UnitZ.itemCraftManager.ItemSelected == Crafter)
		{
			UnitZ.itemCraftManager.CancelCraft();
		}
		if ((bool)CrafterLoader)
		{
			CrafterLoader.SelectCraft(Index, null);
		}
	}

	public void ConfirmCraft()
	{
		if ((bool)UnitZ.itemCraftManager && (bool)UnitZ.playerManager && (bool)UnitZ.playerManager.playingCharacter)
		{
			UnitZ.itemCraftManager.CraftSelected(Crafter);
			UnitZ.itemCraftManager.Craft(UnitZ.playerManager.playingCharacter.inventory);
		}
	}
}
