using UnityEngine;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterDriver : MonoBehaviour
{
	[HideInInspector]
	public Seat DrivingSeat;

	[HideInInspector]
	public string LastSeat;

	[HideInInspector]
	public NetworkView networkViewer;

	[HideInInspector]
	public CharacterSystem character;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		character = GetComponent<CharacterSystem>();
	}

	public void Drive(Vector2 input, bool brake)
	{
		if ((bool)DrivingSeat && DrivingSeat.IsDriver && (bool)DrivingSeat.VehicleRoot)
		{
			DrivingSeat.VehicleRoot.Drive(new Vector2(input.x, input.y), brake);
		}
	}

	public void OutVehicle()
	{
		if (character != null)
		{
			DrivingSeat.GetOut(this);
		}
	}

	public void OnVehicle(string vehicleID, string seatID, bool isSit)
	{
		if ((bool)networkViewer && (Network.isServer || Network.isClient))
		{
			networkViewer.RPC("onVehicle", RPCMode.Others, vehicleID, seatID, isSit);
		}
	}

	[RPC]
	private void onVehicle(string vehicleID, string seatID, bool isSit)
	{
		if (character == null)
		{
			return;
		}
		Vehicle[] array = (Vehicle[])Object.FindObjectsOfType(typeof(Vehicle));
		Vehicle[] array2 = array;
		Vehicle[] array3 = array2;
		foreach (Vehicle vehicle in array3)
		{
			if (!(vehicle.VehicleID == vehicleID))
			{
				continue;
			}
			Seat[] seats = vehicle.Seats;
			Seat[] array4 = seats;
			foreach (Seat seat in array4)
			{
				if (seat.SeatID == seatID)
				{
					seat.OnSeat(this, isSit);
					break;
				}
			}
			break;
		}
	}
}
