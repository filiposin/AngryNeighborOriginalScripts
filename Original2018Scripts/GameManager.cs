using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public string UserName = string.Empty;

	public string Team = string.Empty;

	public string UserID = string.Empty;

	public string CharacterKey = string.Empty;

	[HideInInspector]
	public HostData[] gameList;

	[HideInInspector]
	public bool OfflineMode;

	[HideInInspector]
	public string PlayingLevel;

	[HideInInspector]
	public string CurrentLevel;

	[HideInInspector]
	public bool IsRefreshing;

	[HideInInspector]
	public bool IsPlaying;

	[HideInInspector]
	public int lastLevelPrefix;

	[HideInInspector]
	public string PlayerID = string.Empty;

	private NetworkView networkViewer;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		networkViewer = GetComponent<NetworkView>();
		if ((bool)networkViewer)
		{
			networkViewer.group = 1;
		}
	}

	private void Start()
	{
		PlayerPrefs.SetString("landingpage", Application.loadedLevelName);
		UserName = PlayerPrefs.GetString("user_name");
		gameList = null;
	}

	private void Update()
	{
		if (IsRefreshing && MasterServer.PollHostList().Length > 0)
		{
			IsRefreshing = false;
			gameList = MasterServer.PollHostList();
			Debug.Log("Get data from master server " + gameList.Length);
		}
		CurrentLevel = Application.loadedLevelName;
	}

	public void CreateGame(string startlevel, bool multiplayer)
	{
		PlayerID = "0";
		IsPlaying = false;
		PlayingLevel = startlevel;
		OfflineMode = !multiplayer;
		RestartGame();
		if (multiplayer && (bool)UnitZ.gameServer)
		{
			UnitZ.gameServer.StartServer();
		}
	}

	public void StartGame(string level)
	{
		if (OfflineMode || Network.isServer)
		{
			StartSinglePlayerGame(level);
		}
		else
		{
			StartMultiplayerGame();
		}
	}

	public void StartMultiplayerGame()
	{
		if (!Network.isServer && !OfflineMode)
		{
			UnitZ.gameClient.AttemptConnectToServer();
			IsPlaying = false;
		}
		PlayerPrefs.SetString("user_name", UserName);
	}

	public void StartSinglePlayerGame(string level)
	{
		StartLoadLevel(level, lastLevelPrefix);
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.UpdatePlayerInfo("0", 0, 0, UserName, Team, UnitZ.GameKeyVersion, true);
		}
		PlayerID = "0";
		IsPlaying = true;
		PlayerPrefs.SetString("user_name", UserName);
	}

	public void RestartGame()
	{
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.ClearPlayers();
			UnitZ.playersManager.AddPlayer("0");
		}
		if (UnitZ.playerManager != null)
		{
			UnitZ.playerManager.Reset();
		}
	}

	public void QuitGame()
	{
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.ClearPlayers();
		}
		if (UnitZ.chatLog != null)
		{
			UnitZ.chatLog.Clear();
		}
		if (!Network.isClient && !Network.isServer)
		{
			ClearNetworkGameObject();
			if (Application.loadedLevelName != PlayerPrefs.GetString("landingpage"))
			{
				Application.LoadLevel(PlayerPrefs.GetString("landingpage"));
				Object.Destroy(base.gameObject);
			}
			return;
		}
		if (Network.isServer)
		{
			UnitZ.gameServer.KillServer();
		}
		if (Network.isClient)
		{
			UnitZ.gameClient.Disconnect();
		}
	}

	public void ConnectingDeny()
	{
		Network.Disconnect();
	}

	public void ClearNetworkGameObject()
	{
		Debug.Log("Clear all object");
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = (GameObject)array[i];
			if ((bool)gameObject.GetComponent<NetworkView>() && gameObject.gameObject != base.gameObject)
			{
				Object.Destroy(gameObject.gameObject);
			}
		}
		if (UnitZ.playerManager != null)
		{
			UnitZ.playerManager.Reset();
		}
	}

	public void Refresh()
	{
		MasterServer.RequestHostList(UnitZ.gameServer.ServerName);
		gameList = null;
		IsRefreshing = true;
	}

	private void OnGUI()
	{
		GUI.skin.label.fontSize = 14;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		string text = "Local game";
		if ((bool)UnitZ.gameServer && !UnitZ.gameServer.LanOnly)
		{
			text = "Online";
		}
		if (Network.isServer)
		{
			if ((bool)networkViewer)
			{
				GUI.Label(new Rect(0f, 0f, 800f, 100f), IsPlaying + " - " + text + "  Server : " + UnitZ.gameServer.IPServer + " (" + (Network.connections.Length + 1) + ") Players : ID is :" + networkViewer.owner.ToString() + " Team : " + Team);
			}
		}
		else if (Network.isClient)
		{
			GUI.Label(new Rect(0f, 0f, 800f, 30f), IsPlaying + " - " + text + "  Client : Your ID is :" + PlayerID + " Team : " + Team);
		}
	}

	private void OnLevelWasLoaded()
	{
		Debug.Log(Application.loadedLevelName + " was loaded");
		if (IsPlaying && Application.loadedLevelName == PlayingLevel)
		{
			if (UnitZ.playerManager != null && !UnitZ.playerManager.ManualSpawn)
			{
				UnitZ.playerManager.RequestSpawnPlayer();
			}
			if (Network.isServer)
			{
				UnitZ.gameServer.isActive = true;
			}
			if (UnitZ.sceneSave != null)
			{
				UnitZ.sceneSave.LevelLoaded();
			}
			PlayerPrefs.SetString("StartScene", PlayingLevel);
		}
	}

	public void StartLoadLevel(string level, int levelPrefix)
	{
		PlayingLevel = level;
		if (Network.isServer)
		{
			Debug.Log("Server Load level " + level);
			StartCoroutine(SceneLoadLevel(PlayingLevel, levelPrefix));
		}
		if (Network.isClient)
		{
			Debug.Log("Client Load level " + level);
			StartCoroutine(SceneLoadLevel(PlayingLevel, levelPrefix));
		}
		if (!Network.isServer && !Network.isClient)
		{
			Debug.Log("Single player Load level " + PlayingLevel);
			Application.LoadLevel(PlayingLevel);
		}
	}

	[RPC]
	public IEnumerator SceneLoadLevel(string level, int levelPrefix)
	{
		if (Network.isServer || Network.isClient)
		{
			if (Network.isServer)
			{
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
			}
			Debug.Log("Network : Loading " + level);
			lastLevelPrefix = levelPrefix;
			Network.SetSendingEnabled(0, false);
			Network.isMessageQueueRunning = false;
			Network.SetLevelPrefix(lastLevelPrefix);
			Application.LoadLevel(level);
			yield return null;
			yield return null;
			Network.isMessageQueueRunning = true;
			Network.SetSendingEnabled(0, true);
			GameObject[] gameObjects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			GameObject[] array = gameObjects;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
