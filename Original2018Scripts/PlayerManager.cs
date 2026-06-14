using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public GUISkin skin;

	[HideInInspector]
	public CharacterSystem playingCharacter;

	public float SaveInterval = 5f;

	public bool SavePlayer = true;

	[HideInInspector]
	public bool ManualSpawn;

	[HideInInspector]
	public bool DisabledSpawn;

	private SpectreCamera Spectre;

	private float timeTemp;

	private bool savePlayerTemp;

	private NetworkView networkViewer;

	public GameObject BlackIns;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		savePlayerTemp = SavePlayer;
		if (!skin && (bool)UnitZ.styleManager)
		{
			skin = UnitZ.styleManager.GetSkin(0);
		}
	}

	public void Reset()
	{
		ManualSpawn = false;
		DisabledSpawn = false;
		SavePlayer = savePlayerTemp;
	}

	private void Update()
	{
		if (!(UnitZ.gameManager == null) && UnitZ.gameManager.IsPlaying)
		{
			OnPlaying();
		}
	}

	private void findPlayerCharacter()
	{
		CharacterSystem[] array = (CharacterSystem[])Object.FindObjectsOfType(typeof(CharacterSystem));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsMine)
			{
				playingCharacter = array[i];
				break;
			}
		}
	}

	public void RemovePlayerCharacter(string id)
	{
		if (!Network.isServer)
		{
			return;
		}
		CharacterSystem[] array = (CharacterSystem[])Object.FindObjectsOfType(typeof(CharacterSystem));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].ID == id)
			{
				NetworkView component = array[i].GetComponent<NetworkView>();
				if ((bool)component)
				{
					Network.RemoveRPCs(component.viewID);
					Network.Destroy(array[i].gameObject);
				}
			}
		}
	}

	public void OnPlaying()
	{
		if ((bool)playingCharacter)
		{
			if ((bool)UnitZ.playerSave && Time.time >= timeTemp + SaveInterval)
			{
				timeTemp = Time.time;
				if (SavePlayer)
				{
					UnitZ.playerSave.SavePlayer(playingCharacter);
				}
			}
		}
		else
		{
			findPlayerCharacter();
		}
		if (Spectre != null)
		{
			if (playingCharacter == null)
			{
				Spectre.Active(true);
				return;
			}
			Spectre.Active(false);
			Spectre.LookingAt(playingCharacter.gameObject.transform.position);
			playingCharacter.spectreThis = true;
		}
		else
		{
			Spectre = (SpectreCamera)Object.FindObjectOfType(typeof(SpectreCamera));
		}
	}

	public void RequestSpawnWithTeam(string team)
	{
		Debug.Log("Request Spawn with team :" + team);
		UnitZ.gameManager.Team = team;
		if (Network.isClient)
		{
			if ((bool)networkViewer)
			{
				networkViewer.RPC("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
			}
		}
		else
		{
			InstantiatePlayerObject(UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
		}
	}

	public void RequestSpawnWithTeamSpawner(string team, int spawner)
	{
		Debug.Log("Request spawn with team spawner :" + team + " at " + spawner);
		UnitZ.gameManager.Team = team;
		if (Network.isClient)
		{
			if ((bool)networkViewer)
			{
				networkViewer.RPC("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, spawner);
			}
		}
		else
		{
			InstantiatePlayerObject(UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, spawner);
		}
	}

	public void RequestSpawnPlayer()
	{
		if (Network.isClient)
		{
			if ((bool)networkViewer)
			{
				networkViewer.RPC("RequestSpawn", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
			}
		}
		else
		{
			InstantiatePlayerObject(UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.gameManager.CharacterKey, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.Team, -1);
		}
	}

	private bool InstantiatePlayerObject(string playerID, string userID, string userName, string characterKey, int index, string team, int spawner)
	{
		if (UnitZ.characterManager == null || UnitZ.characterManager.CharacterPresets.Length <= index || index < 0)
		{
			return false;
		}
		CharacterSystem characterPrefab = UnitZ.characterManager.CharacterPresets[index].CharacterPrefab;
		PlayerSpawner[] array = (PlayerSpawner[])Object.FindObjectsOfType(typeof(PlayerSpawner));
		if (spawner < 0 || spawner >= array.Length)
		{
			spawner = Random.Range(0, array.Length);
		}
		if ((bool)characterPrefab && array.Length > 0)
		{
			CharacterSystem component = array[spawner].Spawn(characterPrefab.gameObject).GetComponent<CharacterSystem>();
			component.ReceivePlayerInfo(playerID, team, characterKey, userName, userID);
			if (UnitZ.playerSave != null && component != null)
			{
				string hasKey = userID + "_" + Application.loadedLevelName + "_" + characterKey + "_" + userName;
				UnitZ.playerSave.InstantiateCharacter(component, hasKey);
			}
			if (playerID == UnitZ.gameManager.PlayerID)
			{
				playingCharacter = component;
			}
			Debug.Log("Instantiate character : " + component.CharacterKey);
			MouseLock.MouseLocked = true;
			return true;
		}
		return false;
	}

	[RPC]
	public void RequestSpawn(string playerID, string userID, string userName, string characterKey, int index, string team, int spawner, NetworkMessageInfo info)
	{
		if (InstantiatePlayerObject(playerID, userID, userName, characterKey, index, team, spawner) && (bool)networkViewer)
		{
			networkViewer.RPC("spawnedCallback", info.sender);
		}
	}

	[RPC]
	private void spawnedCallback()
	{
		Debug.Log("your character has been spawn on a server side");
		findPlayerCharacter();
	}

	private void OnGUI()
	{
		if ((bool)skin)
		{
			GUI.skin = skin;
		}
		if (!DisabledSpawn && (bool)UnitZ.gameManager && UnitZ.gameManager.IsPlaying && playingCharacter == null)
		{
			BlackIns.SetActive(false);
			RequestSpawnPlayer();
			BlackIns.SetActive(true);
		}
	}
}
