using UnityEngine;

public class DoorFrame : MonoBehaviour
{
	public Animator animator;

	public bool IsOpen;

	private float timeTemp;

	public float Cooldown = 0.5f;

	public string DoorKey = string.Empty;

	private void Start()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
	}

	public void Access(CharacterSystem character)
	{
		AccessDoor(DoorKey);
	}

	private void AccessDoor(string key)
	{
		if (key == DoorKey && Time.time > timeTemp + Cooldown)
		{
			IsOpen = !IsOpen;
			timeTemp = Time.time;
		}
	}

	private void Update()
	{
		if ((bool)animator)
		{
			animator.SetBool("IsOpen", IsOpen);
		}
		{
			int num = 0;
			if (IsOpen)
			{
				num = 1;
			}
			doorUpdate(num);
		}
	}

	private void accessDoor(string key)
	{
		AccessDoor(key);
	}

	private void doorUpdate(int doorstate)
	{
		switch (doorstate)
		{
		case 0:
			IsOpen = false;
			break;
		case 1:
			IsOpen = true;
			break;
		}
	}
}
