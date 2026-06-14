using UnityEngine;

public class ExitLobby : MonoBehaviour
{
	public void click()
	{
		Application.LoadLevel(0);
		Debug.Log("Load MainMenu....");
	}
}
