using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public int level;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void startGame()
	{
		Application.LoadLevel(level);
	}

	public void exitGame()
	{
		Application.Quit();
	}

	public void LowQuality()
	{
		QualitySettings.SetQualityLevel(0, true);
	}

	public void MediumQuality()
	{
		QualitySettings.SetQualityLevel(2, true);
	}

	public void UltraQuality()
	{
		QualitySettings.SetQualityLevel(4, true);
	}
}
public class Mainmenu : MonoBehaviour
{
	public Texture2D LogoGame;

	[HideInInspector]
	public int PageState;

	public string SceneStart = "sandbox";

	[HideInInspector]
	public GameManager gameManage;

	[HideInInspector]
	public CharacterCreator characterCreate;

	public GUISkin skin;

	private float delta;

	private int pageTemp;

	private Vector2 scrollPosition;

	private void Start()
	{
		delta = 1f;
		Application.targetFrameRate = 140;
		gameManage = (GameManager)Object.FindObjectOfType(typeof(GameManager));
		characterCreate = (CharacterCreator)Object.FindObjectOfType(typeof(CharacterCreator));
		StyleManager styleManager = (StyleManager)Object.FindObjectOfType(typeof(StyleManager));
		if (PlayerPrefs.GetString("StartScene") != string.Empty)
		{
			SceneStart = PlayerPrefs.GetString("StartScene");
		}
		if (!skin && (bool)styleManager)
		{
			skin = styleManager.GetSkin(0);
		}
	}

	private void DrawGameLobby()
	{
		if (!gameManage || gameManage.IsRefreshing)
		{
			return;
		}
		if (gameManage.gameList != null)
		{
			scrollPosition = GUI.BeginScrollView(new Rect(Screen.width / 2 - 275, 50f, 550f, Screen.height - 200), scrollPosition, new Rect(0f, 0f, 500f, 60 * gameManage.gameList.Length));
			for (int i = 0; i < gameManage.gameList.Length; i++)
			{
				HostData hostData = gameManage.gameList[i];
				string text = string.Empty;
				string[] ip = hostData.ip;
				string[] array = ip;
				foreach (string text2 in array)
				{
					text += text2;
				}
				if (GUI.Button(new Rect(0f, i * 60, 550f, 50f), hostData.gameName + " " + text))
				{
					UnitZ.gameClient.GameSelected(hostData);
					characterCreate.OpenCharacter();
					PageState = 1;
				}
			}
			GUI.EndScrollView();
		}
		else
		{
			GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200f, 50f), "No game Found");
		}
	}

	private void OnGUI()
	{
		Screen.lockCursor = false;
		if ((bool)skin)
		{
			GUI.skin = skin;
		}
		GUI.skin.button.fontSize = 17;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		if (SceneStart != "zombieland")
		{
			if (GUI.Button(new Rect(10f, 10f, 150f, 30f), "Zombie Land"))
			{
				SceneStart = "zombieland";
			}
		}
		else
		{
			GUI.Box(new Rect(10f, 10f, 150f, 30f), "Zombie Land");
		}
		if (SceneStart != "sandbox")
		{
			if (GUI.Button(new Rect(170f, 10f, 150f, 30f), "Sandbox"))
			{
				SceneStart = "sandbox";
			}
		}
		else
		{
			GUI.Box(new Rect(170f, 10f, 150f, 30f), "Sandbox");
		}
		if (SceneStart != "training")
		{
			if (GUI.Button(new Rect(330f, 10f, 150f, 30f), "Death Match"))
			{
				SceneStart = "training";
			}
		}
		else
		{
			GUI.Box(new Rect(330f, 10f, 150f, 30f), "Death Match");
		}
		switch (PageState)
		{
		case 0:
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 20) + -100f * delta, 260f, 50f), "Single Player"))
			{
				if ((bool)gameManage)
				{
					gameManage.OfflineMode = true;
					characterCreate.OpenCharacter();
				}
				PageState = 1;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + -100f * delta, 260f, 50f), "Network"))
			{
				if ((bool)gameManage)
				{
					gameManage.OfflineMode = false;
				}
				PageState = 2;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + -100f * delta, 260f, 50f), "Exit"))
			{
				Application.Quit();
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, (float)(Screen.height / 2 - 200) + -300f * delta, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		case 1:
			if (GUI.Button(new Rect(50f + -300f * delta, 50f, 160f, 50f), "Back"))
			{
				PageState = 0;
				if ((bool)UnitZ.gameServer)
				{
					UnitZ.gameServer.KillServer();
				}
			}
			if (!UnitZ.gameClient.isConnecting)
			{
				characterCreate.DrawCharacterCreator();
				if (characterCreate.State == 0 && GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + -100f * delta, 260f, 50f), "Enter world") && (bool)gameManage)
				{
					characterCreate.SetCharacter();
					if (gameManage.OfflineMode)
					{
						gameManage.StartSinglePlayerGame(SceneStart);
					}
					else
					{
						gameManage.StartMultiplayerGame();
					}
				}
			}
			else if (UnitZ.gameClient.isConnecting)
			{
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Box(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 25, 260f, 50f), string.Empty);
				GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 25, 260f, 50f), "Connecting to server..");
			}
			break;
		case 2:
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 20) + 50f * delta, 260f, 50f), "Host Game"))
			{
				PageState = 7;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + 50f * delta, 260f, 50f), "Find Game"))
			{
				PageState = 4;
				if ((bool)gameManage)
				{
					gameManage.Refresh();
				}
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + 50f * delta, 260f, 50f), "Connect IP"))
			{
				PageState = 3;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 200) + 50f * delta, 260f, 50f), "Back"))
			{
				PageState = 0;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, Screen.height / 2 - 200, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		case 3:
			UnitZ.gameServer.Port = int.Parse(GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 180f, 50f), UnitZ.gameServer.Port.ToString()));
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 20, 100f, 50f), "Port");
			UnitZ.gameServer.IPServer = GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 80, 180f, 50f), UnitZ.gameServer.IPServer);
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 80, 100f, 50f), "IP");
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + 50f * delta, 260f, 50f), "Connect"))
			{
				characterCreate.OpenCharacter();
				PageState = 1;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 200) + 50f * delta, 260f, 50f), "Back"))
			{
				PageState = 2;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, Screen.height / 2 - 200, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		case 4:
			GUI.Box(new Rect(Screen.width / 2 - 275, 50f, 550f, Screen.height - 200), string.Empty);
			DrawGameLobby();
			if (GUI.Button(new Rect((float)(Screen.width / 2 - 130) + -300f * delta, Screen.height - 120, 260f, 50f), "Back"))
			{
				PageState = 2;
			}
			if (!gameManage)
			{
				break;
			}
			if (!gameManage.IsRefreshing)
			{
				if (GUI.Button(new Rect(Screen.width / 2 + 170, Screen.height - 120, 105f, 50f), "Refresh"))
				{
					gameManage.Refresh();
				}
			}
			else
			{
				GUI.Label(new Rect(Screen.width / 2 + 170, Screen.height - 120, 105f, 50f), "Refreshing..");
			}
			break;
		case 5:
			UnitZ.gameServer.Port = int.Parse(GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 180f, 50f), UnitZ.gameServer.Port.ToString()));
			SceneStart = GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 30, 180f, 50f), SceneStart);
			GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 70, 300f, 50f), "sandbox , zombieland ,training");
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 30, 100f, 50f), "Level");
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 20, 100f, 50f), "Port");
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + 50f * delta, 260f, 50f), "Host Game"))
			{
				if ((bool)gameManage)
				{
					gameManage.CreateGame(SceneStart, true);
				}
				characterCreate.OpenCharacter();
				PageState = 1;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + 50f * delta, 260f, 50f), "Back"))
			{
				PageState = 7;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, Screen.height / 2 - 200, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		case 6:
			GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 70, 300f, 50f), "sandbox , zombieland ,training");
			SceneStart = GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 30, 180f, 50f), SceneStart);
			UnitZ.gameServer.Port = int.Parse(GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 20, 180f, 50f), UnitZ.gameServer.Port.ToString()));
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 30, 100f, 50f), "Level");
			GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 + 20, 100f, 50f), "Port");
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + 50f * delta, 260f, 50f), "Host Local Game"))
			{
				if ((bool)gameManage)
				{
					UnitZ.gameServer.LanOnly = true;
					gameManage.CreateGame(SceneStart, true);
				}
				characterCreate.OpenCharacter();
				PageState = 1;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + 50f * delta, 260f, 50f), "Back"))
			{
				PageState = 7;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, Screen.height / 2 - 200, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		case 7:
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 20) + 50f * delta, 260f, 50f), "Master Server"))
			{
				PageState = 5;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 80) + 50f * delta, 260f, 50f), "Local Game"))
			{
				PageState = 6;
			}
			if (GUI.Button(new Rect(Screen.width / 2 - 130, (float)(Screen.height / 2 + 140) + 50f * delta, 260f, 50f), "Back"))
			{
				PageState = 2;
			}
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) - (float)LogoGame.width * 0.5f / 2f, Screen.height / 2 - 200, (float)LogoGame.width * 0.5f, (float)LogoGame.height * 0.5f), LogoGame);
			break;
		}
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(0f, Screen.height - 50, Screen.width, 30f), "UnitZ beta | www.hardworkerstudio.com");
	}

	private void Update()
	{
		delta += (0f - delta) / 10f;
		if (pageTemp != PageState)
		{
			delta = 1f;
			pageTemp = PageState;
		}
	}
}
