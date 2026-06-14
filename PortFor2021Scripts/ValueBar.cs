using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour
{
	public RectTransform Bar;

	public RectTransform BarBG;

	public float Value = 50f;

	public float ValueMax = 100f;

	public Text ValueText;

	public string CustomText = string.Empty;

	private void Start()
	{
	}

	private void Update()
	{
		if (Bar != null && BarBG != null)
		{
			float x = BarBG.sizeDelta.x / ValueMax * Value;
			Bar.sizeDelta = new Vector2(x, Bar.sizeDelta.y);
		}
		if ((bool)ValueText)
		{
			ValueText.text = Value.ToString();
			if (CustomText != string.Empty)
			{
				ValueText.text = CustomText;
			}
		}
	}
}
