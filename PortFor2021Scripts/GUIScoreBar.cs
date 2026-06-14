using UnityEngine;
using UnityEngine.UI;

public class GUIScoreBar : MonoBehaviour
{
	public Text Name;

	public Text Kills;

	public Text Deads;

	public PlayerData Player;

	private void Start()
	{
		if ((bool)Name)
		{
			Name.enabled = false;
		}
		if ((bool)Kills)
		{
			Kills.enabled = false;
		}
		if ((bool)Deads)
		{
			Deads.enabled = false;
		}
	}

	private void Update()
	{
		if (Player != null)
		{
			if ((bool)Name)
			{
				Name.text = Player.Name;
				Name.enabled = true;
			}
			if ((bool)Kills)
			{
				Kills.text = Player.Score.ToString();
				Kills.enabled = true;
			}
			if ((bool)Deads)
			{
				Deads.text = Player.Dead.ToString();
				Deads.enabled = true;
			}
		}
	}
}
