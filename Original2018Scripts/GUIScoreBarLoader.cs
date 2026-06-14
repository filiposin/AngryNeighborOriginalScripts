using UnityEngine;

public class GUIScoreBarLoader : MonoBehaviour
{
	public RectTransform Canvas;

	public RectTransform GUIScorePrefab;

	public RectTransform GUITeamPrefab;

	private void Start()
	{
	}

	private void OnEnable()
	{
		DrawScoreboard();
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

	public void DrawScoreboard()
	{
		if (UnitZ.playersManager == null || Canvas == null || GUIScorePrefab == null || UnitZ.playersManager.PlayerList == null)
		{
			return;
		}
		ClearCanvas();
		int num = 0;
		foreach (string team in UnitZ.playersManager.TeamList)
		{
			if (team != string.Empty)
			{
				GameObject gameObject = Object.Instantiate(GUITeamPrefab.gameObject, Vector3.zero, Quaternion.identity);
				gameObject.transform.SetParent(Canvas.transform);
				GUITeamBar component = gameObject.GetComponent<GUITeamBar>();
				RectTransform component2 = gameObject.GetComponent<RectTransform>();
				if ((bool)component2)
				{
					component2.localScale = GUITeamPrefab.gameObject.transform.localScale;
					component2.anchoredPosition = new Vector2(0f, 0f - GUITeamPrefab.sizeDelta.y * (float)num);
				}
				if ((bool)component)
				{
					component.TeamName.text = team;
				}
				num++;
			}
			foreach (PlayerData player in UnitZ.playersManager.PlayerList)
			{
				if (player.IsConnected && team == player.Team)
				{
					GameObject gameObject2 = Object.Instantiate(GUIScorePrefab.gameObject, Vector3.zero, Quaternion.identity);
					gameObject2.transform.SetParent(Canvas.transform);
					GUIScoreBar component3 = gameObject2.GetComponent<GUIScoreBar>();
					RectTransform component4 = gameObject2.GetComponent<RectTransform>();
					if ((bool)component4)
					{
						component4.localScale = GUIScorePrefab.gameObject.transform.localScale;
						component4.anchoredPosition = new Vector2(0f, 0f - GUIScorePrefab.sizeDelta.y * (float)num);
					}
					if ((bool)component3)
					{
						component3.Player = player;
					}
					num++;
				}
			}
		}
		Canvas.sizeDelta = new Vector2(Canvas.sizeDelta.x, GUIScorePrefab.sizeDelta.y * (float)num);
	}

	private void Update()
	{
	}
}
