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


	public void StartServer()
	{
		isActive = false;
		if (LanOnly)
		{
			UseNat = false;
		}
	}

	private void OnPlayerConnected()
	{

	}

	public string GetLocalIPAddress()
	{
		string result = string.Empty;
		IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
		IPAddress[] addressList = hostEntry.AddressList;
		foreach (IPAddress iPAddress in addressList)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				result = iPAddress.ToString();
			}
		}
		return result;
	}
}
