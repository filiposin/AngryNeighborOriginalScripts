using UnityEngine;

public class WoodFind : MonoBehaviour
{
	public HitMark hitmark;

	private void Start()
	{
		hitmark = GetComponent<HitMark>();
	}

	private void Update()
	{
		if ((bool)GameObject.Find("Hammer_FPS(Clone)"))
		{
			hitmark.DamageMult = 0f;
		}
		else
		{
			hitmark.DamageMult = 1f;
		}
	}
}
