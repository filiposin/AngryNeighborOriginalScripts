using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FPSItemThrow : FPSItemEquipment
{
	public bool HoldFire;

	public GameObject Item;

	public float FireRate = 0.1f;

	public int UsingType;

	public ItemData ItemUsed;

	public bool InfinityAmmo;

	public bool OnAnimationEvent;

	public float Force1 = 15f;

	public float Force2 = 5f;

	public AudioClip SoundThrow;

	private CharacterSystem character;

	private FPSController fpsController;

	private float timeTemp;

	private AudioSource audioSource;

	private Animator animator;

	private int throwType;

	private void Start()
	{
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		if ((bool)base.transform.root)
		{
			character = base.transform.root.GetComponent<CharacterSystem>();
			fpsController = base.transform.root.GetComponent<FPSController>();
			if (character == null)
			{
				character = base.transform.root.GetComponentInChildren<CharacterSystem>();
			}
			if (fpsController == null)
			{
				fpsController = base.transform.root.GetComponentInChildren<FPSController>();
			}
		}
		else
		{
			character = base.transform.GetComponent<CharacterSystem>();
			fpsController = base.transform.GetComponent<FPSController>();
		}
		timeTemp = Time.time;
	}

	private void Update()
	{
	}

	private void Use()
	{
		if (!(ItemUsed != null) || InfinityAmmo || !(character != null) || !(character.inventory != null) || character.inventory.CheckItem(ItemUsed, 1))
		{
			if (!OnAnimationEvent)
			{
				OnAction();
			}
			if ((bool)animator)
			{
				animator.SetInteger("shoot_type", UsingType);
				animator.SetTrigger("shoot");
			}
			if (character != null)
			{
				character.AttackAnimation(UsingType);
			}
		}
	}

	public override void Trigger()
	{
		if (HoldFire || !OnFire1)
		{
			if ((bool)character && (bool)fpsController && Time.time > timeTemp + FireRate)
			{
				throwType = 0;
				Use();
				timeTemp = Time.time;
			}
			base.Trigger();
		}
	}

	public override void Trigger2()
	{
		if (HoldFire || !OnFire2)
		{
			if ((bool)character && (bool)fpsController && Time.time > timeTemp + FireRate)
			{
				throwType = 1;
				Use();
				timeTemp = Time.time;
			}
			base.Trigger2();
		}
	}

	public override void OnAction()
	{
		if ((bool)Item)
		{
			if (Network.isServer || Network.isClient)
			{
				GameObject gameObject = (GameObject)Network.Instantiate(Item, base.transform.position, base.transform.rotation, 3);
				DamageBase component = gameObject.GetComponent<DamageBase>();
				if ((bool)component && (bool)character && (bool)UnitZ.gameManager)
				{
					component.OwnerID = character.ID;
					component.OwnerTeam = character.Team;
				}
				if (throwType == 0)
				{
					if ((bool)gameObject.GetComponent<Rigidbody>())
					{
						gameObject.GetComponent<Rigidbody>().AddForce(base.transform.forward * Force1, ForceMode.Impulse);
					}
				}
				else if ((bool)gameObject.GetComponent<Rigidbody>())
				{
					gameObject.GetComponent<Rigidbody>().AddForce(base.transform.forward * Force2, ForceMode.Impulse);
				}
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate(Item, base.transform.position, base.transform.rotation);
				DamageBase component2 = gameObject2.GetComponent<DamageBase>();
				if ((bool)component2 && (bool)character && (bool)UnitZ.gameManager)
				{
					component2.OwnerID = character.ID;
					component2.OwnerTeam = character.Team;
				}
				if (throwType == 0)
				{
					if ((bool)gameObject2.GetComponent<Rigidbody>())
					{
						gameObject2.GetComponent<Rigidbody>().AddForce(base.transform.forward * Force1, ForceMode.Impulse);
					}
				}
				else if ((bool)gameObject2.GetComponent<Rigidbody>())
				{
					gameObject2.GetComponent<Rigidbody>().AddForce(base.transform.forward * Force2, ForceMode.Impulse);
				}
			}
			if ((bool)SoundThrow && (bool)audioSource && audioSource.enabled)
			{
				audioSource.PlayOneShot(SoundThrow);
			}
		}
		if (!(ItemUsed != null) || InfinityAmmo || !(character != null) || !(character.inventory != null) || character.inventory.RemoveItem(ItemUsed, 1))
		{
			base.OnAction();
		}
	}
}
