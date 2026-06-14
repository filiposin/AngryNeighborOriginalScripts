using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TooltipTrade : TooltipInstance
{
	public InputField Number;

	private bool trading;

	private static TooltipTrade tooltip;

	private int number;

	public static TooltipTrade Instance
	{
		get
		{
			if (tooltip == null)
			{
				tooltip = Object.FindObjectOfType<TooltipTrade>();
			}
			return tooltip;
		}
	}

	private void Start()
	{
		tooltip = this;
		trading = false;
		HideTooltip();
	}

	private void Update()
	{
	}

	public void UpdateNumber()
	{
		if (!(Number == null))
		{
			int.TryParse(Number.text, out number);
		}
	}

	public void StartTrade(ItemCollector item, DropStockArea stock)
	{
		if (item != null && !(stock == null) && !(Number == null))
		{
			Number.text = item.Num.ToString();
			trading = true;
			base.gameObject.SetActive(true);
			StartCoroutine(Trading(item, stock));
		}
	}

	public IEnumerator Trading(ItemCollector item, DropStockArea stock)
	{
		while (trading)
		{
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("Trade " + item.Item.ItemName + " to " + number);
		stock.Trade(number);
		Cancel();
	}

	public void Ok()
	{
		trading = false;
	}

	public void Cancel()
	{
		HideTooltip();
	}
}
