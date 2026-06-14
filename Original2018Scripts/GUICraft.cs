using UnityEngine;
using UnityEngine.UI;

public class GUICraft : MonoBehaviour
{
	public Text Name;

	public Image Icon;

	public ItemCrafter Crafter;

	public int Index;

	public GUICraftListLoader CrafterLoader;

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
	}

	private void Update()
	{
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

	public void Craft()
	{
		if ((bool)CrafterLoader)
		{
			CrafterLoader.SelectCraft(Index, Crafter);
		}
	}
}
