using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectSpawn : MonoBehaviour
{
	public string ItemID = string.Empty;

	public string ItemUID = string.Empty;

	public GameObject Item;

	public int Group = 2;

	private void Start()
	{
		if ((bool)Item)
		{
			{
				GameObject gameObject2 = Instantiate(Item, base.transform.position, base.transform.rotation);
				gameObject2.SendMessage("SetItemID", ItemID, SendMessageOptions.DontRequireReceiver);
				gameObject2.SendMessage("SetItemUID", ItemUID, SendMessageOptions.DontRequireReceiver);
			}
		}
        Destroy(base.gameObject);
	}

	public void SetItemID(string id)
	{
		ItemID = id;
	}

	public void SetItemUID(string uid)
	{
		ItemUID = uid;
	}

	public void GenItemUID()
	{
		ItemUID = GetUniqueID();
	}

	private void Update()
	{
	}

	public string GetUniqueID()
	{
		System.Random random = new System.Random();
		DateTime dateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
		double totalSeconds = (DateTime.UtcNow - dateTime).TotalSeconds;
		return string.Format("{0:X}", Convert.ToInt32(totalSeconds)) + "-" + string.Format("{0:X}", Convert.ToInt32(Time.time * 1000000f)) + "-" + string.Format("{0:X}", random.Next(1000000000));
	}
}
