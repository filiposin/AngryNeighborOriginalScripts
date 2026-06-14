using System;

[Serializable]
public class ItemCrafter
{
	public ItemData ItemResult;

	public int NumResult = 1;

	public ItemNeeded[] ItemNeeds;

	public float CraftTime = 2f;
}
