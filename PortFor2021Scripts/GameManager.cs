using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public string UserName = string.Empty;

	public string Team = string.Empty;

	public string UserID = string.Empty;

	public string CharacterKey = string.Empty;

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

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		PlayerPrefs.SetString("landingpage", Application.loadedLevelName);
		UserName = PlayerPrefs.GetString("user_name");
	}

	private void Update()
	{
		if (IsRefreshing)
		{
			IsRefreshing = false;
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
		if (OfflineMode)
		{
			StartSinglePlayerGame(level);
		}
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
		{
			if (Application.loadedLevelName != PlayerPrefs.GetString("landingpage"))
			{
				Application.LoadLevel(PlayerPrefs.GetString("landingpage"));
				Object.Destroy(base.gameObject);
			}
			return;
		}
	}

	public void Refresh()
	{
		IsRefreshing = true;
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
		{
			Debug.Log("Single player Load level " + PlayingLevel);
			Application.LoadLevel(PlayingLevel);
		}
	}

	public IEnumerator SceneLoadLevel(string level, int levelPrefix)
	{
			Debug.Log("Network : Loading " + level);
			lastLevelPrefix = levelPrefix;
			Application.LoadLevel(level);
			yield return null;
			yield return null;
			GameObject[] gameObjects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			GameObject[] array = gameObjects;
			foreach (GameObject gameObject in array)
			{
				gameObject.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
			}
		}
}