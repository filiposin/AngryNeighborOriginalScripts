using UnityEngine;

public class URLSS : MonoBehaviour
{
	private void OnClick()
	{
		Application.OpenURL("market://details?id=com.company.game");
	}
}
