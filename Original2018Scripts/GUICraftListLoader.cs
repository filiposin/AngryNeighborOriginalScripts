using UnityEngine;

public class GUICraftListLoader : MonoBehaviour
{
	public RectTransform Canvas;

	public RectTransform GUICraftPrefab;

	public RectTransform GUICraftingPrefab;

	public RectTransform GUICraftNeed;

	private int indexSelected = -1;

	private void Start()
	{
		SetupCrafterList();
	}

	private void OnEnable()
	{
		SetupCrafterList();
	}

	private void SetupCrafterList()
	{
		if (UnitZ.itemCraftManager == null || Canvas == null || GUICraftPrefab == null)
		{
			return;
		}
		Clear();
		float num = 0f;
		for (int i = 0; i < UnitZ.itemCraftManager.ItemCraftList.Length; i++)
		{
			GameObject gameObject = GUICraftPrefab.gameObject;
			if (i == indexSelected)
			{
				gameObject = GUICraftingPrefab.gameObject;
			}
			GameObject gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			GUICraft component2 = gameObject2.GetComponent<GUICraft>();
			if ((bool)component2)
			{
				component2.Crafter = UnitZ.itemCraftManager.ItemCraftList[i];
				component2.CrafterLoader = this;
				component2.Index = i;
			}
			float num2 = 0f;
			if (i == indexSelected)
			{
				num2 = DrawNeeds(UnitZ.itemCraftManager.ItemCraftList[i], component.sizeDelta.y + num);
			}
			gameObject2.transform.SetParent(Canvas.gameObject.transform);
			component.anchoredPosition = new Vector2(5f, 0f - num);
			component.localScale = gameObject.transform.localScale;
			num += component.sizeDelta.y + num2;
		}
		Canvas.sizeDelta = new Vector2(Canvas.sizeDelta.x, num);
	}

	private float DrawNeeds(ItemCrafter crafter, float offset)
	{
		float num = 0f;
		for (int i = 0; i < crafter.ItemNeeds.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(GUICraftNeed.gameObject, Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(Canvas.gameObject.transform);
			GUIItemDataNeed component = gameObject.GetComponent<GUIItemDataNeed>();
			if ((bool)component)
			{
				component.Item = crafter.ItemNeeds[i].Item;
				component.Need = crafter.ItemNeeds[i].Num;
				if (UnitZ.playersManager != null && UnitZ.playerManager.playingCharacter != null)
				{
					component.Inventory = UnitZ.playerManager.playingCharacter.inventory;
				}
			}
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.anchoredPosition = new Vector2(5f, 0f - (component2.sizeDelta.y * (float)i + offset));
			component2.localScale = GUICraftNeed.gameObject.transform.localScale;
			num += component2.sizeDelta.y;
		}
		return num;
	}

	private void Clear()
	{
		if (Canvas == null)
		{
			return;
		}
		foreach (Transform item in Canvas.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void SelectCraft(int index, ItemCrafter crafter)
	{
		if (indexSelected == index)
		{
			indexSelected = -1;
		}
		else
		{
			indexSelected = index;
		}
		SetupCrafterList();
	}

	private void Update()
	{
	}
}
