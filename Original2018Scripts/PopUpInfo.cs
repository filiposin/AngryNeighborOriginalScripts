using UnityEngine;
using UnityEngine.UI;

public class PopUpInfo : MonoBehaviour
{
	public Text ContentText;

	private void Start()
	{
	}

	public void Close()
	{
		base.gameObject.SetActive(false);
	}
}
