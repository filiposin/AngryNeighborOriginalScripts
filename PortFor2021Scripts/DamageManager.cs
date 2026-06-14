using UnityEngine;

public class DamageManager : MonoBehaviour
{
	public int HP = 100;

	public int HPmax = 100;

	private int Armor;

	private int Armormax = 100;

	public GameObject DeadReplacement;

	public float DeadReplaceLifeTime = 180f;

	public bool isAlive = true;

	public AudioClip[] SoundPain;

	public AudioSource Audiosource;

	[HideInInspector]
	public bool dieByLifeTime;

	[HideInInspector]
	public bool spectreThis;

	[HideInInspector]
	public string Team = string.Empty;

	[HideInInspector]
	public string ID = string.Empty;

	[HideInInspector]
	public string UserID = string.Empty;

	[HideInInspector]
	public string UserName = string.Empty;

	[HideInInspector]
	public string LastHitByID = string.Empty;

	[HideInInspector]
	public bool IsMine;

	private bool initialized;

	private Vector3 directionHit;

	public bool ShowTextWhenDead = true;

	public bool time;

	public bool PhysicsDie;

	private bool isQuitting;

	private void Start()
	{
		Audiosource = GetComponent<AudioSource>();
		initialized = false;
	}

	private void OnApplicationQuit()
	{
		isQuitting = true;
	}

	private void Update()
	{
		DamageUpdate();
	}

	public void DamageUpdate()
	{
		if (isAlive)
		{
			if (HP > HPmax)
			{
				HP = HPmax;
			}
			if (Armor > Armormax)
			{
				Armor = Armormax;
			}
			if (HP <= 0)
			{
				{
					isAlive = false;
					OnDead();
				}
			}
			UpdateData();
		}
		if ((bool)UnitZ.gameManager && UnitZ.gameManager.PlayerID == ID)
		{
			IsMine = true;
		}
		else
		{
			IsMine = false;
		}
	}

	public void ApplyDamage(int damage, Vector3 direction, string attackerID, string team)
	{
		{
			directionHit = direction;
			LastHitByID = attackerID;
			if (Team != team || team == string.Empty)
			{
				HP -= damage;
				{
					applyDamage(damage, direction, attackerID, team);
				}
			}
		}
		if ((bool)Audiosource && SoundPain.Length > 0)
		{
			Audiosource.PlayOneShot(SoundPain[Random.Range(0, SoundPain.Length)]);
		}
	}

	private void applyDamage(int damage, Vector3 direction, string attackerID, string team)
	{
		directionHit = direction;
		LastHitByID = attackerID;
		if (Team != team || team == string.Empty)
		{
			HP -= damage;
		}
	}

	public void DirectDamage(DamagePackage pack)
	{
		ApplyDamage((int)(float)pack.Damage, pack.Direction, pack.ID, pack.Team);
	}

	private void OnDead()
	{
		base.gameObject.SendMessage("OnThisThingDead", SendMessageOptions.DontRequireReceiver);
		{
			if (PhysicsDie)
			{
				base.gameObject.AddComponent<Rigidbody>();
				base.gameObject.layer = 8;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (!isQuitting && (bool)DeadReplacement && !Application.isLoadingLevel)
		{
			GameObject gameObject = Object.Instantiate(DeadReplacement, base.transform.position, Quaternion.identity);
			CopyTransformsRecurse(base.transform, gameObject);
			if (time)
			{
				Object.Destroy(gameObject, DeadReplaceLifeTime);
			}
		}
	}

	public void CopyTransformsRecurse(Transform src, GameObject dst)
	{
		dst.transform.position = src.position;
		dst.transform.rotation = src.rotation;
		if ((bool)dst.GetComponent<Rigidbody>())
		{
			dst.GetComponent<Rigidbody>().AddForce(directionHit, ForceMode.VelocityChange);
		}
		foreach (Transform item in dst.transform)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyTransformsRecurse(transform2, item.gameObject);
			}
		}
	}

	public void InitializeData()
	{
		{
			initializeData(HP);
		}
	}

	private void initializeData(int hp)
	{
		Debug.Log("initialize character: " + base.gameObject.name + " hp:" + hp);
		initialized = true;
		HP = hp;
	}

	private void UpdateData()
	{
		if (initialized)
		{
			updateData(ID, Team, HP);
		}
	}

	private void updateData(string id, string team, int hp)
	{
		HP = hp;
		ID = id;
		Team = team;
	}
}
