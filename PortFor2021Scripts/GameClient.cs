using UnityEngine;

public class GameClient : MonoBehaviour
{
	public float ConnectionTimeOut = 10f;

	[HideInInspector]
	public bool isConnecting;

	[HideInInspector]
	public float Delay;

	private float timeConnecting;

	private void Update()
	{
		if (isConnecting && Time.time > timeConnecting + ConnectionTimeOut)
		{
			isConnecting = false;
		}
	}

	public void AttemptConnectToServer()
	{
		if (UnitZ.playersManager != null)
		{
			UnitZ.playersManager.ClearPlayers();
		}
		{
			isConnecting = true;
			timeConnecting = Time.time;
			Debug.Log("Connecting to : " + UnitZ.gameServer.IPServer);
		}
	}

	private void PlayerConnectedCallback(string playerID, string playingLevel, int levelprefix, string gameKey, bool isPlaying)
	{
		Debug.Log("Callback from server !!");
		UnitZ.gameManager.IsPlaying = isPlaying;
		UnitZ.gameManager.PlayerID = playerID;
		if (isPlaying && gameKey == UnitZ.GameKeyVersion)
		{
			UnitZ.gameManager.StartLoadLevel(playingLevel, levelprefix);
			{
				
			}
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

	public void OnDisconnectedFromServer()
	{
		UnitZ.gameManager.IsPlaying = false;
		if (Application.loadedLevelName != PlayerPrefs.GetString("landingpage"))
		{
			Application.LoadLevel(PlayerPrefs.GetString("landingpage"));
			Object.Destroy(base.gameObject);
		}
		isConnecting = false;
		Debug.Log("Disconnected from server!");
	}
}
