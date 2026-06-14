using UnityEngine;

public class CarControl : MonoBehaviour
{
	public MortorType Type;

	public WheelCollider Wheel_FL;

	public WheelCollider Wheel_FR;

	public WheelCollider Wheel_RL;

	public WheelCollider Wheel_RR;

	public float[] GearRatio;

	public int CurrentGear;

	public float EngineTorque = 600f;

	public float MaxEngineRPM = 3000f;

	public float MinEngineRPM = 1000f;

	public float SteerAngle = 10f;

	public Transform COM;

	public float Speed;

	public float maxSpeed = 150f;

	public AudioSource skidAudio;

	public float EngineRPM;

	private float motorInput;

	public NetworkView networkViewer;

	private Vector2 inputAxis;

	private bool inputBrake;

	public float steerAngleTemp;

	private float latestMotorInput;

	private void Awake()
	{
		networkViewer = GetComponent<NetworkView>();
	}

	private void Start()
	{
		GetComponent<Rigidbody>().centerOfMass = new Vector3(COM.localPosition.x * base.transform.localScale.x, COM.localPosition.y * base.transform.localScale.y, COM.localPosition.z * base.transform.localScale.z);
	}

	public void Controller(Vector2 input, bool brake)
	{
		inputAxis = input;
		inputBrake = brake;
	}

	private void Update()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer))
		{
			if ((bool)GetComponent<Rigidbody>())
			{
				GetComponent<Rigidbody>().isKinematic = false;
			}
			Speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
			GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 100f;
			EngineRPM = (Wheel_FL.rpm + Wheel_FR.rpm) / 2f * GearRatio[CurrentGear];
			ShiftGears();
			motorInput = inputAxis.y;
			GetComponent<AudioSource>().pitch = Mathf.Abs(EngineRPM / MaxEngineRPM) + 1f;
			if (GetComponent<AudioSource>().pitch > 2f)
			{
				GetComponent<AudioSource>().pitch = 2f;
			}
			steerAngleTemp = SteerAngle * inputAxis.x;
			Wheel_FL.steerAngle = steerAngleTemp;
			Wheel_FR.steerAngle = steerAngleTemp;
			if (Speed > maxSpeed)
			{
				Wheel_RL.motorTorque = 0f;
				Wheel_RR.motorTorque = 0f;
				Wheel_FL.motorTorque = 0f;
				Wheel_FR.motorTorque = 0f;
			}
			else
			{
				switch (Type)
				{
				case MortorType.FourWheel:
					Wheel_FL.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					Wheel_FR.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					Wheel_RL.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					Wheel_RR.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					break;
				case MortorType.Front:
					Wheel_FL.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					Wheel_FR.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					break;
				case MortorType.Rear:
					Wheel_RL.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					Wheel_RR.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputAxis.y;
					break;
				}
			}
			if (latestMotorInput != motorInput)
			{
				Wheel_RL.brakeTorque = 10000f;
				Wheel_RR.brakeTorque = 10000f;
			}
			if (motorInput <= 0f)
			{
				Wheel_RL.brakeTorque = 0f;
				Wheel_RR.brakeTorque = 0f;
			}
			else if (motorInput >= 0f)
			{
				Wheel_RL.brakeTorque = 0f;
				Wheel_RR.brakeTorque = 0f;
			}
			if (inputBrake)
			{
				Wheel_RL.brakeTorque = 1000f;
				Wheel_RR.brakeTorque = 1000f;
			}
			if (!inputBrake)
			{
				Wheel_FL.brakeTorque = 0f;
				Wheel_FR.brakeTorque = 0f;
			}
			WheelHit hit;
			Wheel_RR.GetGroundHit(out hit);
			if (Mathf.Abs(hit.sidewaysSlip) > 10f)
			{
				skidAudio.enabled = true;
			}
			else
			{
				skidAudio.enabled = false;
			}
			inputAxis = Vector2.zero;
			inputBrake = false;
			if ((bool)networkViewer && Network.isServer)
			{
				networkViewer.RPC("engineUpdate", RPCMode.Others, GetComponent<AudioSource>().pitch, steerAngleTemp, Wheel_FL.motorTorque, Wheel_FR.motorTorque, Wheel_RL.motorTorque, Wheel_RR.motorTorque);
			}
		}
		if (Network.isClient)
		{
			if ((bool)GetComponent<Rigidbody>())
			{
				GetComponent<Rigidbody>().isKinematic = true;
			}
			Wheel_FL.steerAngle = steerAngleTemp;
			Wheel_FR.steerAngle = steerAngleTemp;
		}
		latestMotorInput = motorInput;
	}

	[RPC]
	public void engineUpdate(float pitch, float steerAngle, float w_FL, float w_FR, float w_RL, float w_RR)
	{
		GetComponent<AudioSource>().pitch = pitch;
		if (GetComponent<AudioSource>().pitch > 2f)
		{
			GetComponent<AudioSource>().pitch = 2f;
		}
		Wheel_FL.motorTorque = w_FL;
		Wheel_FR.motorTorque = w_FR;
		Wheel_RL.motorTorque = w_RL;
		Wheel_RR.motorTorque = w_RR;
		steerAngleTemp = steerAngle;
	}

	private void ShiftGears()
	{
		int currentGear = CurrentGear;
		if (EngineRPM >= MaxEngineRPM)
		{
			for (int i = 0; i < GearRatio.Length; i++)
			{
				if (Wheel_FL.rpm * GearRatio[i] < MaxEngineRPM)
				{
					currentGear = i;
					break;
				}
			}
			CurrentGear = currentGear;
		}
		if (!(EngineRPM <= MinEngineRPM))
		{
			return;
		}
		currentGear = CurrentGear;
		for (int num = GearRatio.Length - 1; num >= 0; num--)
		{
			if (Wheel_FL.rpm * GearRatio[num] > MinEngineRPM)
			{
				currentGear = num;
				break;
			}
		}
		CurrentGear = currentGear;
	}
}
