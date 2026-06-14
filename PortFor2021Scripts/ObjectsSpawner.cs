using System.Collections.Generic;
using UnityEngine;

public class ObjectsSpawner : MonoBehaviour
{
	public bool SpawnOnStart = true;

	public float timeSpawn = 120f;

	public GameObject[] Obj;

	public int ObjMax = 3;

	public Vector3 Offset = new Vector3(0f, 0.1f, 0f);

	public bool PlaceOnGround = true;

	private float timeTemp;

	private List<GameObject> itemList = new List<GameObject>();

	private int ObjectsNumber;

	private void Start()
	{
		if (SpawnOnStart)
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		ObjectExistCheck();
		if (ObjectsNumber >= ObjMax)
		{
			return;
		}
		if (Obj.Length > 0)
		{
			GameObject gameObject = Obj[Random.Range(0, Obj.Length)];
			GameObject gameObject2 = null;
			Vector3 position = DetectGround(base.transform.position + new Vector3(Random.Range(-(int)(base.transform.localScale.x / 2f), (int)(base.transform.localScale.x / 2f)), 0f, Random.Range((int)((0f - base.transform.localScale.z) / 2f), (int)(base.transform.localScale.z / 2f))));
			{
				gameObject2 = Instantiate(gameObject.gameObject, position, Quaternion.identity);
			}
			if ((bool)gameObject2)
			{
				itemList.Add(gameObject2);
			}
		}
		timeTemp = Time.time;
	}

	private void ObjectExistCheck()
	{
		ObjectsNumber = 0;
		foreach (GameObject item in itemList)
		{
			if (item != null)
			{
				ObjectsNumber++;
			}
		}
	}

	private void Update()
	{
		if (Time.time > timeTemp + timeSpawn)
		{
			Spawn();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.2f);
		Gizmos.DrawWireCube(base.transform.position, base.transform.localScale);
	}

	private Vector3 DetectGround(Vector3 position)
	{
		RaycastHit hitInfo;
		if (PlaceOnGround && Physics.Raycast(position, -Vector3.up, out hitInfo, 1000f))
		{
			return hitInfo.point + Offset;
		}
		return position;
	}
}
