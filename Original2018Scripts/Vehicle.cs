using UnityEngine;

[RequireComponent(typeof(CarControl))]
public class Vehicle : DamageManager
{
	public Seat[] Seats;

	public string VehicleName;

	public string VehicleID;

	[HideInInspector]
	public bool incontrol;

	public bool ShowInfo;

	private void Awake()
	{
		networkViewer = GetComponent<NetworkView>();
		if (Network.isServer)
		{
			VehicleID = "V_" + base.transform.GetInstanceID();
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (Seats.Length <= 0)
		{
			Component[] componentsInChildren = GetComponentsInChildren(typeof(Seat));
			Seats = new Seat[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Seats[i] = componentsInChildren[i].GetComponent<Seat>();
			}
		}
	}

	private void Start()
	{
	}

	public virtual void Pickup(CharacterSystem character)
	{
		if (!character || (!character.IsMine && (Network.isClient || Network.isServer)))
		{
			return;
		}
		CharacterDriver component = character.GetComponent<CharacterDriver>();
		if (!component)
		{
			return;
		}
		for (int i = 0; i < Seats.Length; i++)
		{
			if (Seats[i].passenger == null)
			{
				Debug.Log("Pick up " + character.name);
				Seats[i].GetIn(component);
				break;
			}
		}
	}

	private Seat FindOpenSeat()
	{
		for (int i = 0; i < Seats.Length; i++)
		{
			if (!Seats[i].Active)
			{
				return Seats[i];
			}
		}
		return null;
	}

	public virtual void Drive(Vector2 input, bool brake)
	{
	}

	public virtual void Ejected()
	{
	}

	public void UpdateFunction()
	{
		UpdateTrasform();
		DamageUpdate();
		if (Network.isServer && (bool)networkViewer)
		{
			networkViewer.RPC("UpdateVehicle", RPCMode.Others, VehicleID);
		}
	}

	private void Update()
	{
		UpdateFunction();
		UpdateDriver();
	}

	public void UpdateDriver()
	{
		for (int i = 0; i < Seats.Length; i++)
		{
			if (Seats[i].IsDriver && Seats[i].passenger != null)
			{
				return;
			}
		}
		incontrol = false;
	}

	public void UpdateTrasform()
	{
		if (Network.isServer && (bool)networkViewer)
		{
			networkViewer.RPC("UpdateTransform", RPCMode.Others, base.transform.position, base.transform.rotation);
		}
	}

	public void FixedUpdate()
	{
		ShowInfo = false;
	}

	public void GetInfo()
	{
		ShowInfo = true;
	}

	private void OnGUI()
	{
		if (ShowInfo)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.gameObject.transform.position);
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(vector.x, (float)Screen.height - vector.y, 200f, 60f), "Get in\n" + VehicleName);
		}
	}

	[RPC]
	public void UpdateTransform(Vector3 position, Quaternion rotation)
	{
		base.transform.position = Vector3.Lerp(base.transform.position, position, 0.5f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, 0.5f);
	}

	[RPC]
	public void UpdateVehicle(string id)
	{
		VehicleID = id;
	}

	public void UpdateSeat(CharacterDriver driver, Seat seat, bool sit)
	{
		driver.OnVehicle(VehicleID, seat.SeatID, sit);
	}
}
