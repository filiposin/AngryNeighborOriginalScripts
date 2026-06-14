using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameServer : MonoBehaviour
{
	public string ServerName = "UZ_coop";

	public int Port = 25000;

	public int MaxPlayer = 32;

	public string IPServer = "127.0.0.1";

	public string FacilitatorIP = string.Empty;

	public string MasterServerIP = string.Empty;

	public int MasterServerPort = 23466;

	public int FacilitatorPort = 50005;

	public bool LanOnly;

	public bool UseNat;

	[HideInInspector]
	public bool isActive;

	private NetworkView networkViewer;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		if (FacilitatorIP != string.Empty)
		{
			Network.natFacilitatorIP = FacilitatorIP;
		}
		if (MasterServerIP != string.Empty)
		{
			MasterServer.ipAddress = MasterServerIP;
			Network.natFacilitatorIP = MasterServerIP;
		}
		if (MasterServerPort >= 0)
		{
			MasterServer.port = MasterServerPort;
		}
		if (FacilitatorPort >= 0)
		{
			Network.natFacilitatorPort = FacilitatorPort;
		}
	}

	public void StartServer()
	{
		isActive = false;
		if (LanOnly)
		{
			UseNat = false;
		}
		else
		{
			UseNat = !Network.HavePublicAddress();
		}
		Network.InitializeServer(MaxPlayer, Port, UseNat);
		if (!LanOnly)
		{
			MasterServer.RegisterHost(ServerName, "World " + SystemInfo.deviceName + "  " + SystemInfo.deviceType);
		}
	}

	public void KillServer()
	{
		isActive = false;
		Network.SetLevelPrefix(0);
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
		Network.Disconnect();
		if (Network.isServer)
		{
			MasterServer.UnregisterHost();
		}
		UnitZ.playersManager.PlayerList.Clear();
		Debug.Log("Kill Server");
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.isServer)
		{
			Debug.Log("Player " + player.ipAddress + ":" + player.port + " is connected");
			if (UnitZ.chatLog != null)
			{
				UnitZ.chatLog.AddLog("<color=gray>" + player.ipAddress + " is joined!</color>");
			}
			if ((bool)networkViewer)
			{
				networkViewer.RPC("PlayerConnectedCallback", player, player.ToString(), UnitZ.gameManager.CurrentLevel, UnitZ.gameManager.lastLevelPrefix, UnitZ.GameKeyVersion, isActive);
			}
		}
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		if (Network.isServer)
		{
			if (UnitZ.chatLog != null)
			{
				UnitZ.chatLog.AddLog("<color=gray>" + player.ipAddress + " is disconnected!</color>");
			}
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
			UnitZ.playerManager.RemovePlayerCharacter(player.ToString());
		}
	}

	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server registered");
		}
	}

	[RPC]
	private void PlayerRegisteration(NetworkPlayer player, string name)
	{
		if ((bool)UnitZ.playersManager)
		{
			UnitZ.playersManager.AddPlayer(player.ToString());
		}
	}

	private void OnServerInitialized()
	{
		if ((bool)networkViewer)
		{
			UnitZ.gameManager.PlayerID = networkViewer.owner.ToString();
		}
		Debug.Log("Server initialized!");
	}

	public string GetLocalIPAddress()
	{
		string result = string.Empty;
		IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress[] addressList = hostEntry.AddressList;
		IPAddress[] array = addressList;
		foreach (IPAddress iPAddress in array)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				result = iPAddress.ToString();
			}
		}
		return result;
	}

	[RPC]
	private void PingTest(float time, NetworkMessageInfo info)
	{
		if ((bool)networkViewer)
		{
			networkViewer.RPC("PingReceived", info.sender, time);
		}
	}
}
