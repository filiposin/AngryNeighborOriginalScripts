using UnityEngine;

public class CharacterHUDCanvas : PanelsManager
{
	public GUIIStockItemLoader SecondItemLoader;

	private CharacterLiving living;

	public GameObject Canvas;

	public ValueBar HPbar;

	public ValueBar FoodBar;

	public ValueBar WaterBar;

	private void Start()
	{
	}

	private void Awake()
	{
		if (Pages.Length > 0)
		{
			ClosePanel(Pages[0]);
		}
	}

	private void InputController()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			MouseLock.MouseLocked = !TogglePanelByName("Inventory");
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			MouseLock.MouseLocked = !TogglePanelByName("InGameMenu");
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			TogglePanelByName("Scoreboard");
		}
		if (IsPanelOpened("InGameMenu") || IsPanelOpened("Inventory") || IsPanelOpened("Craft") || IsPanelOpened("InventoryTrade"))
		{
			MouseLock.MouseLocked = false;
		}
		else
		{
			MouseLock.MouseLocked = true;
		}
	}

	private void Update()
	{
		if (UnitZ.playerManager == null || Canvas == null)
		{
			return;
		}
		if (UnitZ.playerManager.playingCharacter == null)
		{
			living = null;
			Canvas.gameObject.SetActive(false);
			return;
		}
		Canvas.gameObject.SetActive(true);
		InputController();
		if ((bool)HPbar)
		{
			HPbar.Value = UnitZ.playerManager.playingCharacter.HP;
			HPbar.ValueMax = UnitZ.playerManager.playingCharacter.HPmax;
		}
		if ((bool)FoodBar && (bool)living)
		{
			FoodBar.Value = living.Hungry;
			FoodBar.ValueMax = living.HungryMax;
		}
		if ((bool)WaterBar && (bool)living)
		{
			WaterBar.Value = living.Water;
			WaterBar.ValueMax = living.WaterMax;
		}
		if (living == null)
		{
			living = UnitZ.playerManager.playingCharacter.GetComponent<CharacterLiving>();
		}
	}

	public void OpenSecondInventory(CharacterInventory inventory, string type)
	{
		if (IsPanelOpened("InventoryTrade"))
		{
			ClosePanelByName("InventoryTrade");
			return;
		}
		SecondItemLoader.OpenInventory(inventory, type);
		OpenPanelByName("InventoryTrade");
	}

	public void CloseSecondInventory()
	{
		ClosePanelByName("InventoryTrade");
	}
}
