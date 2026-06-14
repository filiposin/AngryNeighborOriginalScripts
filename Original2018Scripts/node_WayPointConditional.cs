using UnityEngine;

public class node_WayPointConditional : MonoBehaviour
{
	[Tooltip("Set true if this node is used in a conditional setting. Check out the conditionalWayPointDemo scene for an example")]
	public bool conditionalWayPoint;

	[Tooltip("use this to id custom wayPoints for unique behaviours. Typically only used on conditinal wayPoints")]
	public string wayPointName;

	[HideInInspector]
	public bool atThisWayPoint;

	[HideInInspector]
	public Transform[] thingsWayPoints;

	[HideInInspector]
	public GameObject thingAtThisWayPoint;

	[HideInInspector]
	public Transform nextWayPointConditional;

	[HideInInspector]
	public bool moveToNextWayPoint;

	private void Update()
	{
		if (!atThisWayPoint)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Transform[] array = thingsWayPoints;
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				if (transform.GetComponent<node_WayPointConditional>().wayPointName == "red" && transform != base.transform)
				{
					nextWayPointConditional = transform;
					moveToNextWayPoint = true;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Transform[] array3 = thingsWayPoints;
			Transform[] array4 = array3;
			foreach (Transform transform2 in array4)
			{
				if (transform2.GetComponent<node_WayPointConditional>().wayPointName == "blue" && transform2 != base.transform)
				{
					nextWayPointConditional = transform2;
					moveToNextWayPoint = true;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Transform[] array5 = thingsWayPoints;
			Transform[] array6 = array5;
			foreach (Transform transform3 in array6)
			{
				if (transform3.GetComponent<node_WayPointConditional>().wayPointName == "orange" && transform3 != base.transform)
				{
					nextWayPointConditional = transform3;
					moveToNextWayPoint = true;
				}
			}
		}
		if (!Input.GetKeyDown(KeyCode.Alpha4))
		{
			return;
		}
		Transform[] array7 = thingsWayPoints;
		Transform[] array8 = array7;
		foreach (Transform transform4 in array8)
		{
			if (transform4.GetComponent<node_WayPointConditional>().wayPointName == "green" && transform4 != base.transform)
			{
				nextWayPointConditional = transform4;
				moveToNextWayPoint = true;
			}
		}
	}
}
