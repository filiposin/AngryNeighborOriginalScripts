using UnityEngine;

public class GameClient : MonoBehaviour
{
	public float ConnectionTimeOut = 10f;

	[HideInInspector]
	public bool isConnecting;

	[HideInInspector]
	public HostData ServerSelected;

	[HideInInspector]
	public float Delay;

	private NetworkView networkViewer;

	private float timeConnecting;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
	}

	private void Update()
	{
		if (isConnecting && Time.time > timeConnecting + ConnectionTimeOut)
		{
			isConnecting = false;
		}
		if (Network.isClient && UnitZ.gameManager.IsPlaying)
		{
			networkViewer.RPC("UpdateMyInfo", RPCMode.Server, UnitZ.gameManager.PlayerID, UnitZ.gameManager.UserName, UnitZ.gameManager.Team, true);
		}
	}

	public void GameSelected(HostData game)
	{
		ServerSelected = game;
	}

	public void AttemptConnectToServer()
	{
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.ClearPlayers();
		}
		if (ServerSelected != null)
		{
			if (ServerSelected != null)
			{
				Network.Connect(ServerSelected);
				isConnecting = true;
				timeConnecting = Time.time;
				Debug.Log("Connecting to : " + ServerSelected.gameName);
			}
			else
			{
				Debug.Log("No server selected");
			}
		}
		else
		{
			Network.Connect(UnitZ.gameServer.IPServer, UnitZ.gameServer.Port);
			isConnecting = true;
			timeConnecting = Time.time;
			Debug.Log("Connecting to : " + UnitZ.gameServer.IPServer);
		}
	}

	public void Disconnect()
	{
		Network.Disconnect();
		ServerSelected = null;
	}

	[RPC]
	private void PlayerConnectedCallback(string playerID, string playingLevel, int levelprefix, string gameKey, bool isPlaying)
	{
		Debug.Log("Callback from server !!");
		UnitZ.gameManager.IsPlaying = isPlaying;
		UnitZ.gameManager.PlayerID = playerID;
		if (isPlaying && gameKey == UnitZ.GameKeyVersion)
		{
			UnitZ.gameManager.StartLoadLevel(playingLevel, levelprefix);
			if ((bool)networkViewer)
			{
				networkViewer.RPC("UpdatePlayerInfo", RPCMode.Server, playerID, 0, 0, UnitZ.gameManager.UserID, UnitZ.gameManager.Team, UnitZ.GameKeyVersion, true);
			}
		}
		else
		{
			UnitZ.gameManager.IsPlaying = false;
			Debug.Log("Server is not ready!");
			if (UnitZ.popup != null)
			{
				UnitZ.popup.ShowPopup("Host is not ready!");
			}
			if (gameKey != UnitZ.GameKeyVersion && UnitZ.popup != null)
			{
				UnitZ.popup.ShowPopup("Your client is wrong version");
			}
			isConnecting = false;
			Network.Disconnect();
		}
	}

	private void OnConnectedToServer()
	{
		Debug.Log("Connected to server!");
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.UpdatePlayerInfo("0", 0, 0, UnitZ.gameManager.UserName, UnitZ.gameManager.Team, UnitZ.GameKeyVersion, true);
		}
		isConnecting = false;
	}

	private void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Could not connect to server: " + error);
		if (UnitZ.popup != null)
		{
			UnitZ.popup.ShowPopup("Could not connect to server: " + error);
		}
		isConnecting = false;
	}

	private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		Debug.Log("Could not connect to master server: " + info);
		if (UnitZ.popup != null)
		{
			UnitZ.popup.ShowPopup("Could not connect to master server: " + info);
		}
		isConnecting = false;
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (UnitZ.chatLog != null)
		{
			UnitZ.chatLog.Clear();
		}
		UnitZ.gameManager.ClearNetworkGameObject();
		UnitZ.gameManager.IsPlaying = false;
		if (Application.loadedLevelName != PlayerPrefs.GetString("landingpage"))
		{
			Application.LoadLevel(PlayerPrefs.GetString("landingpage"));
			Object.Destroy(base.gameObject);
		}
		Network.SetLevelPrefix(0);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
		ServerSelected = null;
		isConnecting = false;
		Debug.Log("Disconnected from server!");
	}

	private void PingRequest()
	{
		float time = Time.time;
		if ((bool)networkViewer)
		{
			networkViewer.RPC("PingTest", RPCMode.Server, time);
		}
	}

	[RPC]
	private void PingReceived(float time)
	{
		Delay = Time.time - time;
		Debug.Log(Delay);
	}
}
