using UnityEngine;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterDriver : MonoBehaviour
{
	[HideInInspector]
	public Seat DrivingSeat;

	[HideInInspector]
	public string LastSeat;

	[HideInInspector]
	public CharacterSystem character;

	private void Start()
	{
		character = GetComponent<CharacterSystem>();
	}

	public void Drive(Vector2 input, bool brake)
	{

	}

	public void OutVehicle()
	{
		if (character != null)
		{
			DrivingSeat.GetOut(this);
		}
	}


	private void onVehicle(string vehicleID, string seatID, bool isSit)
	{
		if (character == null)
		{
			return;
		}
	}
}