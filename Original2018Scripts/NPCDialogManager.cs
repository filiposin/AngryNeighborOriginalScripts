using UnityEngine;

public class NPCDialogManager : MonoBehaviour
{
	public GUIPlayerItemLoader PlayerItemLoader;

	public GUIIStockItemLoader SecondItemLoader;

	public CharacterHUDCanvas HUD;

	private void Start()
	{
		HUD = (CharacterHUDCanvas)Object.FindObjectOfType(typeof(CharacterHUDCanvas));
		if ((bool)HUD)
		{
			HUD.OpenPanelByName("InventoryStock");
		}
	}

	public void OpenStock(ItemStocker stock)
	{
		SecondItemLoader.OpenInventory(stock.inventory, "Stock");
		if ((bool)HUD)
		{
			HUD.OpenPanelByName("InventoryStock");
		}
	}

	public void CloseStock()
	{
		if ((bool)HUD)
		{
			HUD.ClosePanelByName("InventoryStock");
		}
	}
}
