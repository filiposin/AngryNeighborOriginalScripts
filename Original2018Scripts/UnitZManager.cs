using UnityEngine;

public class UnitZManager : MonoBehaviour
{
	public string GameKeyVersion = "first_build";

	private void Awake()
	{
		UnitZ.gameManager = (GameManager)Object.FindObjectOfType(typeof(GameManager));
		UnitZ.gameServer = (GameServer)Object.FindObjectOfType(typeof(GameServer));
		UnitZ.gameClient = (GameClient)Object.FindObjectOfType(typeof(GameClient));
		UnitZ.characterManager = (CharacterManager)Object.FindObjectOfType(typeof(CharacterManager));
		UnitZ.itemManager = (ItemManager)Object.FindObjectOfType(typeof(ItemManager));
		UnitZ.itemCraftManager = (ItemCrafterManager)Object.FindObjectOfType(typeof(ItemCrafterManager));
		UnitZ.playerManager = (PlayerManager)Object.FindObjectOfType(typeof(PlayerManager));
		UnitZ.playersManager = (PlayersManager)Object.FindObjectOfType(typeof(PlayersManager));
		UnitZ.playerSave = (PlayerSave)Object.FindObjectOfType(typeof(PlayerSave));
		UnitZ.scoreManager = (ScoreManager)Object.FindObjectOfType(typeof(ScoreManager));
		UnitZ.sceneSave = (SceneSave)Object.FindObjectOfType(typeof(SceneSave));
		UnitZ.chatLog = (ChatLog)Object.FindObjectOfType(typeof(ChatLog));
		UnitZ.styleManager = (StyleManager)Object.FindObjectOfType(typeof(StyleManager));
		UnitZ.popup = (Popup)Object.FindObjectOfType(typeof(Popup));
		UnitZ.sceneManager = (SceneManager)Object.FindObjectOfType(typeof(SceneManager));
		UnitZ.Hud = (CharacterHUDCanvas)Object.FindObjectOfType(typeof(CharacterHUDCanvas));
		UnitZ.GameKeyVersion = GameKeyVersion;
	}
}
