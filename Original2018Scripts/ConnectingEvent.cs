using UnityEngine;
using UnityEngine.UI;

public class ConnectingEvent : MonoBehaviour
{
	public Text TextInfo;

	public float Timeout = 10f;

	public GameObject Preloading;

	private float timeTemp;

	private bool conneted;

	private void OnEnable()
	{
		if ((bool)Preloading)
		{
			Preloading.SetActive(true);
		}
		conneted = false;
		timeTemp = Time.time;
		if ((bool)TextInfo)
		{
			TextInfo.text = "Connecting to server";
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!conneted && Time.time >= Timeout + timeTemp)
		{
			if ((bool)TextInfo)
			{
				TextInfo.text = "Connecting Time out";
			}
			if ((bool)Preloading)
			{
				Preloading.SetActive(false);
			}
		}
	}

	private void OnFailedToConnect(NetworkConnectionError error)
	{
		if ((bool)Preloading)
		{
			Preloading.SetActive(false);
		}
		if ((bool)TextInfo)
		{
			TextInfo.text = error.ToString();
		}
	}

	private void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		if ((bool)Preloading)
		{
			Preloading.SetActive(false);
		}
		if ((bool)TextInfo)
		{
			TextInfo.text = info.ToString();
		}
	}

	private void OnConnectedToServer()
	{
		conneted = true;
		if ((bool)TextInfo)
		{
			TextInfo.text = "Connected! Loading GameOBject";
		}
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		conneted = true;
		if ((bool)Preloading)
		{
			Preloading.SetActive(false);
		}
		if ((bool)TextInfo)
		{
			TextInfo.text = "Disconnected!";
		}
	}
}
