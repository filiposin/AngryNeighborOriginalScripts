using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : PanelsManager
{
	public string SceneStart = "zombieland";

	public CharacterCreatorCanvas characterCreator;

	public Text CharacterName;

	public GameObject Preloader;

	public GameObject loader;

	public GameObject loading;

	private void Start()
	{
		Application.targetFrameRate = 140;
		characterCreator = (CharacterCreatorCanvas)Object.FindObjectOfType(typeof(CharacterCreatorCanvas));
		if (PlayerPrefs.GetString("StartScene") != string.Empty)
		{
			SceneStart = PlayerPrefs.GetString("StartScene");
		}
	}

	private void Update()
	{
		if ((bool)CharacterName && (bool)UnitZ.gameManager)
		{
			CharacterName.text = UnitZ.gameManager.UserName;
		}
		if ((bool)UnitZ.gameClient && UnitZ.gameClient.isConnecting && (bool)Preloader)
		{
			Preloader.SetActive(UnitZ.gameClient.isConnecting);
		}
	}

	public void LevelSelected(string name)
	{
		SceneStart = name;
		PlayerPrefs.SetString("StartScene", SceneStart);
	}

	public void ConnectIP()
	{
		OpenPanelByName("LoadCharacter");
	}

	public void HostGame()
	{
		if ((bool)UnitZ.gameManager)
		{
			UnitZ.gameManager.CreateGame(SceneStart, true);
		}
	}

	public void UseMasterServer(bool masterserver)
	{
		if ((bool)UnitZ.gameServer)
		{
			UnitZ.gameServer.LanOnly = !masterserver;
		}
	}

	public void StartSinglePlayer()
	{
		if ((bool)UnitZ.gameManager)
		{
			loader.SetActive(true);
			loading.SetActive(true);
			UnitZ.gameManager.CreateGame(SceneStart, false);
			OpenPanelByName("LoadCharacter");
		}
	}

	public void StartNetworkGame()
	{
		if ((bool)UnitZ.gameManager)
		{
			UnitZ.gameManager.CreateGame(SceneStart, true);
			OpenPanelByName("LoadCharacter");
		}
	}

	public void EnterWorld()
	{
		if ((bool)UnitZ.gameManager)
		{
			if ((bool)characterCreator)
			{
				characterCreator.SetCharacter();
			}
			UnitZ.gameManager.StartGame(SceneStart);
		}
		OpenPanelByName("Connecting");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
