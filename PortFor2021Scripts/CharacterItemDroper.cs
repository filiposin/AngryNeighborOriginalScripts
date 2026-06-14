using UnityEngine;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterItemDroper : MonoBehaviour
{
	public GameObject Backpack;

	private CharacterSystem character;

	private void Start()
	{
		character = GetComponent<CharacterSystem>();
	}

	private void Update()
	{
	}

	public void DropItem()
	{
		if (!character || !Backpack)
		{
			return;
		}
		ItemBackpack itemBackpack = null;
		{
			GameObject gameObject2 = Object.Instantiate(Backpack.gameObject, base.transform.position, Quaternion.identity);
			itemBackpack = gameObject2.GetComponent<ItemBackpack>();
		}
		if (!itemBackpack)
		{
			return;
		}
		foreach (ItemCollector item in character.inventory.Items)
		{
			if (character != null && UnitZ.itemManager != null)
			{
				ItemData itemData = UnitZ.itemManager.CloneItemData(item.Item);
				if (itemData != null)
				{
					ItemCollector itemCollector = new ItemCollector();
					itemCollector.Item = itemData;
					itemCollector.Num = item.Num;
					itemCollector.NumTag = item.NumTag;
					itemCollector.Active = true;
					itemBackpack.AddItem(itemCollector);
				}
			}
		}
	}
}
