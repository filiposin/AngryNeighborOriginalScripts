using UnityEngine;

public class NextScene : MonoBehaviour
{
	public string SceneName = "mainmenu";

	private void Start()
	{
		Application.LoadLevel(SceneName);
	}
}
