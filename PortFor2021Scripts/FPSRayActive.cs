using System;
using System.Collections.Generic;
using UnityEngine;

public class FPSRayActive : MonoBehaviour
{
	public bool Sorting;

	public string[] IgnoreTag = new string[1] { "Player" };

	public string[] DestroyerTag = new string[1] { "scene" };

	[Header("Слой для игнорирования интеракта, посмотри туториал если не знаешь как его реализовать")]
    //Короче это наш слой которая будет игнорировать интеракт просто накиньте слой на дом или на любой кубик через который игрок не должен брать предметы
    public string WallLayerName = "Project";
    private int _wallLayer = -1;


	private void Start()
	{
		_wallLayer = LayerMask.NameToLayer(WallLayerName);
        if (_wallLayer == -1)
            Debug.LogWarning($"[FPSRayActive] Layer '{WallLayerName}' не найден. Блокировка интеракта работать не будет.");
	}

	public void ShootRayOnce(Vector3 origin, Vector3 direction, string id, string team)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, direction, out hitInfo, 100f) && hitInfo.collider.gameObject != base.gameObject)
		{
			DamagePackage damagePackage = default(DamagePackage);
			damagePackage.Damage = 50;
			damagePackage.Normal = hitInfo.normal;
			damagePackage.Direction = direction;
			damagePackage.Position = hitInfo.point;
			damagePackage.ID = id;
			damagePackage.Team = team;
			hitInfo.collider.SendMessage("OnHit", damagePackage, SendMessageOptions.DontRequireReceiver);
		}
	}


    public void CheckingRay(Vector3 origin, Vector3 direction)
    {
        float maxDistance = 3f;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
        if (hits == null || hits.Length == 0) return;
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!h.collider) continue;
            if (_wallLayer != -1 && h.collider.gameObject.layer == _wallLayer)
                break;

            h.collider.SendMessage("GetInfo", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ActiveRay(Vector3 origin, Vector3 direction)
    {
        float maxDistance = 3f;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, maxDistance);
        if (hits == null || hits.Length == 0) return;

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (!h.collider) continue;

            if (_wallLayer != -1 && h.collider.gameObject.layer == _wallLayer)
                break;

            h.collider.SendMessage("Pickup", GetComponent<CharacterSystem>(), SendMessageOptions.DontRequireReceiver);
        }
    }

	public bool ShootRay(Vector3 origin, Vector3[] direction, int damage, float size, int hitmax, string id, string team)
	{
		bool result = false;
		DamagePackage damagePackage = default(DamagePackage);
		for (int i = 0; i < direction.Length; i++)
		{
			int num = damage;
			int num2 = 0;
			int num3 = 0;
			List<casthit> list = new List<casthit>();
			RaycastHit[] array = Physics.RaycastAll(origin, direction[i], size);
			for (int j = 0; j < array.Length; j++)
			{
				if ((bool)array[j].collider && tagCheck(array[j].collider.gameObject) && array[j].collider.gameObject != base.gameObject && (((bool)array[j].collider.transform.root && array[j].collider.transform.root != base.gameObject.transform.root && array[j].collider.transform.root.gameObject != base.gameObject) || array[j].collider.transform.root == null))
				{
					num3++;
					casthit item = default(casthit);
					item.distance = Vector3.Distance(origin, array[j].point);
					item.index = j;
					item.name = array[j].collider.name;
					list.Add(item);
				}
			}
			RaycastHit[] array2 = new RaycastHit[num3];
			list.Sort((casthit x, casthit y) => x.distance.CompareTo(y.distance));
			for (int k = 0; k < list.Count; k++)
			{
				array2[k] = array[list[k].index];
			}
			for (int l = 0; l < array2.Length; l++)
			{
				if (num2 >= hitmax)
				{
					break;
				}
				RaycastHit raycastHit = array2[l];
				damagePackage.Damage = damage;
				damagePackage.Normal = raycastHit.normal;
				damagePackage.Direction = direction[i];
				damagePackage.Position = raycastHit.point;
				damagePackage.ID = id;
				damagePackage.Team = team;
				raycastHit.collider.SendMessage("OnHit", damagePackage, SendMessageOptions.DontRequireReceiver);
				result = true;
				num2++;
				if (num2 >= hitmax || tagDestroyerCheck(raycastHit.collider.gameObject))
				{
					break;
				}
				int num4 = (int)((float)num * 0.75f);
				num = num4;
			}
		}
		return result;
	}

	public bool ShootRayTest(Vector3 origin, Vector3[] direction, int damage, float size, int hitmax, string id, string team)
	{
		bool result = false;
		for (int i = 0; i < direction.Length; i++)
		{
			int num = 0;
			int num2 = 0;
			List<casthit> list = new List<casthit>();
			RaycastHit[] array = Physics.RaycastAll(origin, direction[i], size);
			for (int j = 0; j < array.Length; j++)
			{
				if ((bool)array[j].collider && tagCheck(array[j].collider.gameObject) && array[j].collider.gameObject != base.gameObject && (((bool)array[j].collider.transform.root && array[j].collider.transform.root.gameObject != base.gameObject) || array[j].collider.transform.root == null))
				{
					num2++;
					casthit item = default(casthit);
					item.distance = Vector3.Distance(origin, array[j].point);
					item.index = j;
					item.name = array[j].collider.name;
					list.Add(item);
				}
			}
			RaycastHit[] array2 = new RaycastHit[num2];
			list.Sort((casthit x, casthit y) => x.distance.CompareTo(y.distance));
			for (int k = 0; k < list.Count; k++)
			{
				array2[k] = array[list[k].index];
			}
			for (int l = 0; l < array2.Length; l++)
			{
				if (num >= hitmax)
				{
					break;
				}
				RaycastHit raycastHit = array2[l];
				result = true;
				num++;
				if (num >= hitmax || tagDestroyerCheck(raycastHit.collider.gameObject))
				{
					break;
				}
			}
		}
		return result;
	}

	public bool Overlap(Vector3 origin, Vector3 forward, int damage, float size, float dot, string id, string team)
	{
		bool result = false;
		Collider[] array = Physics.OverlapSphere(origin, size);
		Collider[] array2 = array;
		DamagePackage damagePackage = default(DamagePackage);
		foreach (Collider collider in array2)
		{
			if ((bool)collider && collider.gameObject != base.gameObject && collider.gameObject.transform.root != base.gameObject.transform)
			{
				Debug.Log(collider.gameObject.transform.root.name);
				Vector3 normalized = (collider.transform.position - origin).normalized;
				float num = Vector3.Dot(normalized, forward);
				if (num >= dot)
				{
					damagePackage.Damage = damage;
					damagePackage.Normal = normalized;
					damagePackage.Direction = forward;
					damagePackage.Position = collider.gameObject.transform.position;
					damagePackage.ID = id;
					damagePackage.Team = team;
					collider.GetComponent<Collider>().SendMessage("OnHit", damagePackage, SendMessageOptions.DontRequireReceiver);
					result = true;
				}
			}
		}
		return result;
	}

	public bool OverlapTest(Vector3 origin, Vector3 forward, int damage, float size, float dot, string id, string team)
	{
		bool result = false;
		Collider[] array = Physics.OverlapSphere(origin, size);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((bool)collider && collider.gameObject != base.gameObject && collider.gameObject.transform.root != base.gameObject)
			{
				Vector3 normalized = (collider.transform.position - origin).normalized;
				float num = Vector3.Dot(normalized, forward);
				if (num >= dot)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private bool tagDestroyerCheck(GameObject obj)
	{
		for (int i = 0; i < DestroyerTag.Length; i++)
		{
			if (obj.CompareTag(DestroyerTag[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool tagCheck(GameObject obj)
	{
		for (int i = 0; i < IgnoreTag.Length; i++)
		{
			if (obj.CompareTag(IgnoreTag[i]))
			{
				return false;
			}
		}
		return true;
	}
}
