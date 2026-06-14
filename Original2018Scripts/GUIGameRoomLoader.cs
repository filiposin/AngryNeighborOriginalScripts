using System.Collections;
using UnityEngine;

public class GUIGameRoomLoader : MonoBehaviour
{
	public RectTransform GameRoomPrefab;

	public RectTransform Canvas;

	public float TimeOut = 10f;

	private float timeTemp;

	private void Start()
	{
		Refresh();
	}

	private void ClearCanvas()
	{
		if (Canvas == null)
		{
			return;
		}
		foreach (Transform item in Canvas.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		if ((bool)UnitZ.gameManager)
		{
			UnitZ.gameManager.Refresh();
		}
		StartCoroutine(LoadGameRoom());
	}

	public void DrawGameLobby()
	{
		if (UnitZ.gameManager == null || Canvas == null || GameRoomPrefab == null || UnitZ.gameManager.gameList == null)
		{
			return;
		}
		ClearCanvas();
		for (int i = 0; i < UnitZ.gameManager.gameList.Length; i++)
		{
			HostData hostData = UnitZ.gameManager.gameList[i];
			string text = string.Empty;
			string[] ip = hostData.ip;
			string[] array = ip;
			foreach (string text2 in array)
			{
				text += text2;
			}
			GameObject gameObject = Object.Instantiate(GameRoomPrefab.gameObject, Vector3.zero, Quaternion.identity);
			gameObject.transform.SetParent(Canvas.transform);
			GUIGameRoom component = gameObject.GetComponent<GUIGameRoom>();
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			if ((bool)component2)
			{
				component2.anchoredPosition = new Vector2(5f, 0f - GameRoomPrefab.sizeDelta.y * (float)i);
				component2.localScale = GameRoomPrefab.gameObject.transform.localScale;
			}
			if ((bool)component.RoomName)
			{
				component.RoomName.text = hostData.gameName + " " + text;
				component.hostData = hostData;
			}
		}
		RectTransform component3 = Canvas.gameObject.GetComponent<RectTransform>();
		component3.sizeDelta = new Vector2(component3.sizeDelta.x, GameRoomPrefab.sizeDelta.y * (float)UnitZ.gameManager.gameList.Length);
	}

	private IEnumerator LoadGameRoom()
	{
		timeTemp = Time.time;
		bool timeOut = false;
		if (!UnitZ.gameManager)
		{
			yield break;
		}
		while (UnitZ.gameManager.IsRefreshing && !timeOut)
		{
			if (Time.time > timeTemp + TimeOut)
			{
				timeOut = true;
			}
			yield return new WaitForEndOfFrame();
		}
		timeTemp = Time.time;
		DrawGameLobby();
	}
}
