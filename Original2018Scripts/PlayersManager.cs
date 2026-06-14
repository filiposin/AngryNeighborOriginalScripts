using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
	public float VersionCheckDelay = 5f;

	public List<PlayerData> PlayerList = new List<PlayerData>();

	public List<string> TeamList = new List<string>();

	private NetworkView networkViewer;

	private void Start()
	{
		ClearPlayers();
		networkViewer = GetComponent<NetworkView>();
	}

	public void ClearPlayers()
	{
		PlayerList.Clear();
		TeamList.Clear();
	}

	public PlayerData GetPlayerData(string ID)
	{
		foreach (PlayerData player in PlayerList)
		{
			if (player.ID == ID)
			{
				return player;
			}
		}
		return null;
	}

	private void Update()
	{
		if (networkViewer == null || !Network.isServer || !UnitZ.gameServer.isActive)
		{
			return;
		}
		foreach (PlayerData player in PlayerList)
		{
			if (player != null)
			{
				networkViewer.RPC("UpdatePlayerInfo", RPCMode.Others, player.ID, player.Score, player.Dead, player.Name, player.Team, player.GameKey, player.IsConnected);
			}
		}
		UpdateMyInfo("0", UnitZ.gameManager.UserName, UnitZ.gameManager.Team, true);
		foreach (PlayerData player2 in PlayerList)
		{
			if (player2.ID != "0")
			{
				player2.IsConnected = false;
			}
			else
			{
				player2.IsConnected = true;
			}
		}
		NetworkPlayer[] connections = Network.connections;
		for (int i = 0; i < connections.Length; i++)
		{
			NetworkPlayer target = connections[i];
			foreach (PlayerData player3 in UnitZ.playersManager.PlayerList)
			{
				if (player3.ID == target.ToString() || player3.ID == "0")
				{
					player3.IsConnected = true;
					if (player3.GameKey != UnitZ.GameKeyVersion)
					{
						Network.CloseConnection(target, true);
						player3.IsConnected = false;
					}
				}
			}
		}
	}

	[RPC]
	public void AddPlayer(string id)
	{
		PlayerData playerData = new PlayerData();
		playerData.Dead = 0;
		playerData.ID = id;
		playerData.Name = string.Empty;
		playerData.Team = string.Empty;
		playerData.IsConnected = true;
		PlayerList.Add(playerData);
	}

	private void AddTeam(string team)
	{
		foreach (string team2 in TeamList)
		{
			if (team == team2)
			{
				return;
			}
		}
		TeamList.Add(team);
	}

	[RPC]
	public void UpdateMyInfo(string id, string name, string team, bool isconnected)
	{
		foreach (PlayerData player in PlayerList)
		{
			if (id == player.ID)
			{
				player.Name = name;
				player.Team = team;
				player.IsConnected = isconnected;
				AddTeam(team);
				break;
			}
		}
	}

	[RPC]
	public void UpdatePlayerInfo(string id, int score, int dead, string name, string team, string gameKey, bool isconnected)
	{
		bool flag = false;
		foreach (PlayerData player in PlayerList)
		{
			if (player.ID == id)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			AddPlayer(id);
		}
		foreach (PlayerData player2 in PlayerList)
		{
			if (id == player2.ID)
			{
				player2.Score = score;
				player2.Dead = dead;
				player2.Name = name;
				player2.Team = team;
				player2.GameKey = gameKey;
				player2.ConnectedTime = Time.time;
				player2.IsConnected = isconnected;
				AddTeam(team);
				break;
			}
		}
	}
}
