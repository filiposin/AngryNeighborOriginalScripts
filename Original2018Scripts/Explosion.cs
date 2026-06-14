using System.Collections.Generic;
using UnityEngine;

public class Explosion : DamageBase
{
	public float Duration = 3f;

	public float Force = 100f;

	public float Radius = 30f;

	public int Damage = 100;

	public string[] BlockerTag = new string[1] { "Scene" };

	private void Start()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius);
		Collider[] array2 = array;
		DamagePackage damagePackage = default(DamagePackage);
		Collider[] array3 = array2;
		foreach (Collider collider in array3)
		{
			float num = Vector3.Distance(base.transform.position, collider.transform.position);
			float num2 = 1f - 1f / Radius * num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			int num3 = 0;
			List<casthit> list = new List<casthit>();
			RaycastHit[] array4 = Physics.RaycastAll(base.transform.position, (collider.transform.position - base.transform.position).normalized, num + 0.2f);
			for (int j = 0; j < array4.Length; j++)
			{
				if ((bool)array4[j].collider && array4[j].collider.gameObject != base.gameObject)
				{
					num3++;
					casthit item = default(casthit);
					item.distance = Vector3.Distance(base.transform.position, array4[j].point);
					item.index = j;
					item.name = array4[j].collider.name;
					list.Add(item);
				}
			}
			RaycastHit[] array5 = new RaycastHit[num3];
			list.Sort((casthit x, casthit y) => x.distance.CompareTo(y.distance));
			for (int k = 0; k < list.Count; k++)
			{
				array5[k] = array4[list[k].index];
			}
			int num4 = 0;
			bool flag = true;
			for (int l = 0; l < array5.Length; l++)
			{
				if (num4 >= 10)
				{
					break;
				}
				if (array5[l].collider.gameObject == collider.GetComponent<Collider>().gameObject)
				{
					flag = true;
					break;
				}
				if (tagDestroyerCheck(array5[l].collider.tag))
				{
					flag = false;
					break;
				}
				num4++;
			}
			if (flag)
			{
				if ((bool)collider && (bool)collider.GetComponent<Rigidbody>())
				{
					collider.GetComponent<Rigidbody>().AddExplosionForce(Force, base.transform.position, Radius, 3f);
				}
				damagePackage.Damage = (int)((float)Damage * num2);
				damagePackage.Normal = collider.transform.forward;
				damagePackage.Direction = (collider.transform.position - base.transform.position).normalized * Force;
				damagePackage.Position = collider.transform.position;
				damagePackage.ID = OwnerID;
				damagePackage.Team = OwnerTeam;
				collider.GetComponent<Collider>().SendMessage("OnHit", damagePackage, SendMessageOptions.DontRequireReceiver);
			}
		}
		Object.Destroy(base.gameObject, Duration);
	}

	private bool tagDestroyerCheck(string tag)
	{
		for (int i = 0; i < BlockerTag.Length; i++)
		{
			if (BlockerTag[i] == tag)
			{
				return true;
			}
		}
		return false;
	}
}
