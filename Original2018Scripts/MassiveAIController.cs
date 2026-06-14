using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class MassiveAIController : MonoBehaviour
{
	public string PlayerTag = "Player";

	public AICharacterController[] ai;

	private void Start()
	{
	}

	private void Update()
	{
		bool flag = !Network.isServer && !Network.isClient;
		if (!Network.isServer && !flag)
		{
			return;
		}
		ai = (AICharacterController[])Object.FindObjectsOfType(typeof(AICharacterController));
		for (int i = 0; i < ai.Length; i++)
		{
			ai[i].DistanceAttack = ai[i].character.PrimaryWeaponDistance;
			float num = Vector3.Distance(ai[i].PositionTarget, ai[i].gameObject.transform.position);
			Vector3 vector = ai[i].PositionTarget - ai[i].transform.position;
			Quaternion b = Quaternion.LookRotation(vector);
			b.x = 0f;
			b.z = 0f;
			float t = ai[i].TurnSpeed * Time.time;
			ai[i].gameObject.transform.rotation = Quaternion.Lerp(ai[i].gameObject.transform.rotation, b, t);
			if (ai[i].ObjectTarget != null)
			{
				ai[i].PositionTarget = ai[i].ObjectTarget.transform.position;
				if (ai[i].aiTime <= 0)
				{
					ai[i].aiState = Random.Range(0, 4);
					ai[i].aiTime = Random.Range(10, 100);
				}
				else
				{
					ai[i].aiTime--;
				}
				if (num <= ai[i].DistanceAttack)
				{
					if (ai[i].aiState == 0 || ai[i].BrutalMode)
					{
						ai[i].Attack(vector);
					}
				}
				else if (num <= ai[i].DistanceMoveTo)
				{
					ai[i].gameObject.transform.rotation = Quaternion.Lerp(ai[i].gameObject.transform.rotation, b, t);
				}
				else
				{
					ai[i].ObjectTarget = null;
					if (ai[i].aiState == 0)
					{
						ai[i].aiState = 1;
						ai[i].aiTime = Random.Range(10, 500);
						ai[i].PositionTarget = ai[i].positionTemp + new Vector3(Random.Range(0f - ai[i].PatrolRange, ai[i].PatrolRange), 0f, Random.Range(0f - ai[i].PatrolRange, ai[i].PatrolRange));
					}
				}
			}
			else
			{
				float num2 = float.MaxValue;
				for (int j = 0; j < ai[i].TargetTag.Length; j++)
				{
					GameObject[] array = GameObject.FindGameObjectsWithTag(ai[i].TargetTag[j]);
					for (int k = 0; k < array.Length; k++)
					{
						float num3 = Vector3.Distance(array[k].gameObject.transform.position, ai[i].gameObject.transform.position);
						if (num3 <= num2 && (num3 <= ai[i].DistanceMoveTo || num3 <= ai[i].DistanceAttack || ai[i].RushMode) && ai[i].ObjectTarget != array[k].gameObject)
						{
							num2 = num3;
							ai[i].ObjectTarget = array[k].gameObject;
						}
					}
				}
				if (ai[i].aiState == 0)
				{
					ai[i].aiState = 1;
					ai[i].aiTime = Random.Range(10, 200);
					ai[i].PositionTarget = ai[i].positionTemp + new Vector3(Random.Range(0f - ai[i].PatrolRange, ai[i].PatrolRange), 0f, Random.Range(0f - ai[i].PatrolRange, ai[i].PatrolRange));
				}
				if (ai[i].aiTime <= 0)
				{
					ai[i].aiState = Random.Range(0, 4);
					ai[i].aiTime = Random.Range(10, 200);
				}
				else
				{
					ai[i].aiTime--;
				}
			}
			if (!flag)
			{
				ai[i].character.networkViewer.RPC("MoveToPosition", RPCMode.All, ai[i].PositionTarget);
			}
			else
			{
				ai[i].character.MoveToPosition(ai[i].PositionTarget);
			}
		}
	}
}
