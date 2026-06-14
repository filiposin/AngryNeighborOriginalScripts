using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FPSItemUsing : FPSItemEquipment
{
	public bool HoldFire;

	public GameObject Item;

	public float FireRate = 0.1f;

	public int UsingType;

	public ItemData ItemUsed;

	public bool InfinityAmmo;

	public bool OnAnimationEvent;

	public AudioClip SoundUse;

	private CharacterSystem character;

	private FPSController fpsController;

	private float timeTemp;

	private AudioSource audioSource;

	private Animator animator;

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
		Debug.Log("Use item");
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
				Use();
				timeTemp = Time.time;
			}
			base.Trigger();
		}
	}

	public override void OnAction()
	{
		if ((bool)Item)
		{
			if (Network.isServer || Network.isClient)
			{
				GameObject gameObject = (GameObject)Network.Instantiate(Item, base.transform.position, base.transform.rotation, 3);
				gameObject.transform.parent = character.transform;
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate(Item, base.transform.position, base.transform.rotation);
				gameObject2.transform.parent = character.transform;
			}
			if ((bool)SoundUse && (bool)audioSource && audioSource.enabled)
			{
				audioSource.PlayOneShot(SoundUse);
			}
		}
		if (!(ItemUsed != null) || InfinityAmmo || !(character != null) || !(character.inventory != null) || character.inventory.RemoveItem(ItemUsed, 1))
		{
			base.OnAction();
		}
	}
}
