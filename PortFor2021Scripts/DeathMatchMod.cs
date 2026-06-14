using UnityEngine;

public class DeathMatchMod : MonoBehaviour
{
	public Texture2D TeamA;

	public Texture2D TeamB;

	public GUISkin skin;

	private void Awake()
	{
		if ((bool)UnitZ.playerManager)
		{
			UnitZ.playerManager.ManualSpawn = true;
			UnitZ.playerManager.DisabledSpawn = true;
			UnitZ.playerManager.SavePlayer = false;
		}
	}

	private void Start()
	{
		StyleManager styleManager = (StyleManager)Object.FindObjectOfType(typeof(StyleManager));
		if (!skin && (bool)styleManager)
		{
			skin = styleManager.GetSkin(0);
		}
	}

	private void Update()
	{
		if (!(UnitZ.playerManager == null) && UnitZ.playerManager.playingCharacter == null)
		{
			MouseLock.MouseLocked = false;
		}
	}

	private void OnGUI()
	{
		if (UnitZ.playerManager == null)
		{
			return;
		}
		if ((bool)skin)
		{
			GUI.skin = skin;
		}
		if (!UnitZ.playerManager.playingCharacter)
		{
			GUI.BeginGroup(new Rect(Screen.width / 2 - 400, Screen.height / 2 - 200, 800f, 400f));
			if (GUI.Button(new Rect(50f, 0f, 300f, 400f), TeamA))
			{
				UnitZ.playerManager.RequestSpawnWithTeamSpawner("PUPPY", 0);
			}
			if (GUI.Button(new Rect(450f, 0f, 300f, 400f), TeamB))
			{
				UnitZ.playerManager.RequestSpawnWithTeamSpawner("KITTY", 1);
			}
			GUI.EndGroup();
		}
	}
}
