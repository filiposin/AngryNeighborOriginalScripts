using UnityEngine;

public class Popup : MonoBehaviour
{
	public PopUpInfo PopupObject;

	private void Start()
	{
	}

	public void ShowPopup(string text)
	{
		PopupObject.gameObject.SetActive(true);
		PopupObject.ContentText.text = text;
	}
}
