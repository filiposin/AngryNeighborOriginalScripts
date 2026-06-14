using UnityEngine;
using UnityEngine.UI;

public class GUIGameRoom : MonoBehaviour
{
	public Text RoomName;

	public HostData hostData;

	private void Start()
	{
	}

	public void JoinRoom()
	{
		if ((bool)UnitZ.gameClient)
		{
			UnitZ.gameClient.GameSelected(hostData);
			PanelsManager panelsManager = (PanelsManager)Object.FindObjectOfType(typeof(PanelsManager));
			if ((bool)panelsManager)
			{
				panelsManager.OpenPanelByName("LoadCharacter");
			}
		}
	}
}
