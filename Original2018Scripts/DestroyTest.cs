using UnityEngine;

public class DestroyTest : MonoBehaviour
{
	public float duration = 5f;

	private float temp;

	private void Start()
	{
		temp = Time.time;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (Time.time > temp + duration)
		{
			Remove();
		}
	}

	private void Remove()
	{
		Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
		Network.Destroy(GetComponent<NetworkView>().viewID);
	}
}
