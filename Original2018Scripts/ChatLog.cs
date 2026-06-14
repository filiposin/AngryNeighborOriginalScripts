using UnityEngine;

public class ChatLog : MonoBehaviour
{
	private GameManager gameManage;

	public string Log;

	public Color TextColor = Color.white;

	public bool ActiveChat;

	public float ShowTextDuration = 5f;

	private float timeTemp;

	private bool showLog;

	private string chattext = string.Empty;

	private void Start()
	{
		gameManage = (GameManager)Object.FindObjectOfType(typeof(GameManager));
	}

	public void AddLog(string text)
	{
		if ((Network.isClient || Network.isServer) && (bool)GetComponent<NetworkView>())
		{
			GetComponent<NetworkView>().RPC("SendChatMessage", RPCMode.All, text);
		}
	}

	[RPC]
	private void SendChatMessage(string text)
	{
		Log = Log + "\n" + text;
		timeTemp = Time.time;
		showLog = true;
	}

	private void Update()
	{
		if (!gameManage)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			ActiveChat = !ActiveChat;
			if (ActiveChat)
			{
				timeTemp = Time.time;
				showLog = true;
			}
		}
		if (showLog && Time.time >= timeTemp + ShowTextDuration)
		{
			showLog = false;
		}
	}

	public void Clear()
	{
		Log = string.Empty;
	}

	private void OnGUI()
	{
		if (!gameManage || !gameManage.IsPlaying)
		{
			return;
		}
		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = TextColor;
		GUI.skin.label.alignment = TextAnchor.LowerLeft;
		if (showLog)
		{
			GUI.Label(new Rect(10f, 10f, Screen.width, 200f), Log);
		}
		if (ActiveChat)
		{
			timeTemp = Time.time;
			GUI.SetNextControlName("Chattext");
			chattext = GUI.TextField(new Rect(10f, 210f, 200f, 20f), chattext);
			if (Event.current != null && Event.current.keyCode == KeyCode.Return && chattext != string.Empty)
			{
				AddLog("<color=yellow>" + PlayerPrefs.GetString("user_name") + " : </color>" + chattext);
				ActiveChat = false;
				chattext = string.Empty;
			}
			GUI.FocusControl("Chattext");
		}
	}
}
