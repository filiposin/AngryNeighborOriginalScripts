using UnityEngine;

public class SceneSave : MonoBehaviour
{
	public bool AutoSave = true;

	public float SaveInterval = 3f;

	public bool ClearEveryplay;

	private ItemManager itemManager;

	private float timeTemp;

	private void Start()
	{
		itemManager = (ItemManager)Object.FindObjectOfType(typeof(ItemManager));
		timeTemp = Time.time;
	}

	private void Update()
	{
		if (AutoSave && Time.time > timeTemp + SaveInterval)
		{
			SaveObjectPlacing();
			timeTemp = Time.time;
		}
	}

	public void LevelLoaded()
	{
		if (!ClearEveryplay)
		{
			LoadObjectPlacing();
		}
	}

	public void SaveObjectPlacing()
	{
		{
			ObjectPlacing[] array = (ObjectPlacing[])Object.FindObjectsOfType(typeof(ObjectPlacing));
			string loadedLevelName = Application.loadedLevelName;
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i].ItemID + ",";
				text2 = text2 + array[i].ItemUID + ",";
				string text5 = text3;
				text3 = text5 + array[i].transform.position.x + "," + array[i].transform.position.y + "," + array[i].transform.position.z + "|";
				text5 = text4;
				text4 = text5 + array[i].transform.rotation.x + "," + array[i].transform.rotation.y + "," + array[i].transform.rotation.z + "," + array[i].transform.rotation.w + "|";
			}
			PlayerPrefs.SetString(loadedLevelName + "OBJID", text);
			PlayerPrefs.SetString(loadedLevelName + "OBJUID", text2);
			PlayerPrefs.SetString(loadedLevelName + "OBJPOS", text3);
			PlayerPrefs.SetString(loadedLevelName + "OBJROT", text4);
		}
	}

	public void LoadObjectPlacing()
	{
		string loadedLevelName = Application.loadedLevelName;
		if (!itemManager)
		{
			return;
		}
		string @string = PlayerPrefs.GetString(loadedLevelName + "OBJID");
		string string2 = PlayerPrefs.GetString(loadedLevelName + "OBJUID");
		string string3 = PlayerPrefs.GetString(loadedLevelName + "OBJPOS");
		string string4 = PlayerPrefs.GetString(loadedLevelName + "OBJROT");
		string[] array = @string.Split(","[0]);
		string[] array2 = string2.Split(","[0]);
		string[] array3 = string3.Split("|"[0]);
		string[] array4 = string4.Split("|"[0]);
		Vector3[] array5 = new Vector3[array.Length];
		Quaternion[] array6 = new Quaternion[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != string.Empty)
			{
				string[] array7 = array3[i].Split(","[0]);
				if (array7.Length > 2)
				{
					Vector3 zero = Vector3.zero;
					float.TryParse(array7[0], out zero.x);
					float.TryParse(array7[1], out zero.y);
					float.TryParse(array7[2], out zero.z);
					array5[i] = zero;
				}
				string[] array8 = array4[i].Split(","[0]);
				if (array8.Length > 3)
				{
					Quaternion identity = Quaternion.identity;
					float.TryParse(array8[0], out identity.x);
					float.TryParse(array8[1], out identity.y);
					float.TryParse(array8[2], out identity.z);
					float.TryParse(array8[3], out identity.w);
					array6[i] = identity;
				}
				string itemuid = string.Empty;
				if (i < array2.Length && array2.Length > 0)
				{
					itemuid = array2[i];
				}
				itemManager.DirectPlacingObject(array[i], itemuid, array5[i], array6[i]);
			}
		}
	}

	public void ClearObjectPlacing()
	{
		string loadedLevelName = Application.loadedLevelName;
		PlayerPrefs.SetString(loadedLevelName + "OBJID", string.Empty);
		PlayerPrefs.SetString(loadedLevelName + "OBJPOS", string.Empty);
		PlayerPrefs.SetString(loadedLevelName + "OBJROT", string.Empty);
	}
}
