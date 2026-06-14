using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FPSItemPlacing : FPSItemEquipment
{
	public bool HoldFire;

	public GameObject Item;

	public GameObject ItemIndicator;

	public float FireRate = 0.1f;

	public int UsingType;

	public ItemData ItemUsed;

	public bool InfinityAmmo;

	public bool OnAnimationEvent;

	public bool PlacingNormal = true;

	public AudioClip SoundPlaced;

	public float Ranged = 4f;

	public string[] KeyPair = new string[1] { string.Empty };

	private CharacterSystem character;

	private FPSController fpsController;

	private float timeTemp;

	private AudioSource audioSource;

	private Animator animator;

	private GameObject preplacingObject;

	private GameObject objectToSnap;

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
		if ((bool)ItemIndicator)
		{
			preplacingObject = Object.Instantiate(ItemIndicator.gameObject, base.transform.position, ItemIndicator.transform.rotation);
		}
	}

	private void OnDestroy()
	{
		if ((bool)preplacingObject)
		{
			Object.Destroy(preplacingObject);
		}
	}

	private void Update()
	{
		if (!(preplacingObject != null))
		{
			return;
		}
		RaycastHit raycastHit = GoundPlacing();
		if (raycastHit.distance != 0f)
		{
			preplacingObject.SetActive(true);
			if (objectToSnap != null)
			{
				preplacingObject.transform.position = objectToSnap.transform.position;
				preplacingObject.transform.rotation = objectToSnap.transform.rotation;
				return;
			}
			preplacingObject.transform.position = raycastHit.point;
			if (PlacingNormal)
			{
				preplacingObject.transform.rotation = Quaternion.LookRotation(raycastHit.normal);
			}
		}
		else
		{
			preplacingObject.SetActive(false);
		}
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
				Use();
				timeTemp = Time.time;
			}
			base.Trigger();
		}
	}

	public override void OnAction()
	{
		RaycastHit raycastHit = GoundPlacing();
		if (raycastHit.distance != 0f)
		{
			if ((bool)Item)
			{
				Vector3 position = raycastHit.point;
				Quaternion rotation = Item.gameObject.transform.rotation;
				if (PlacingNormal)
				{
					rotation = Quaternion.LookRotation(raycastHit.normal);
				}
				if (objectToSnap != null)
				{
					position = objectToSnap.transform.position;
					rotation = objectToSnap.transform.rotation;
				}
				if (Network.isServer || Network.isClient)
				{
					GameObject gameObject = (GameObject)Network.Instantiate(Item, position, rotation, 2);
					gameObject.SendMessage("SetItemID", ItemID, SendMessageOptions.DontRequireReceiver);
					gameObject.SendMessage("GenItemUID", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					GameObject gameObject2 = Object.Instantiate(Item, position, rotation);
					gameObject2.SendMessage("SetItemID", ItemID, SendMessageOptions.DontRequireReceiver);
					gameObject2.SendMessage("GenItemUID", SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)SoundPlaced && (bool)audioSource && audioSource.enabled)
				{
					audioSource.PlayOneShot(SoundPlaced);
				}
			}
			if (ItemUsed != null && !InfinityAmmo && character != null && character.inventory != null && !character.inventory.RemoveItem(ItemUsed, 1))
			{
				return;
			}
		}
		base.OnAction();
	}

	private RaycastHit GoundPlacing()
	{
		float ranged = Ranged;
		RaycastHit[] array = Physics.RaycastAll(base.transform.position, base.transform.forward, ranged);
		for (int i = 0; i < array.Length; i++)
		{
			PlacingArea component = array[i].collider.GetComponent<PlacingArea>();
			if ((bool)component && component.KeyPairChecker(KeyPair))
			{
				if (component.Snap)
				{
					objectToSnap = component.gameObject;
				}
				if ((bool)array[i].collider && (bool)component)
				{
					return array[i];
				}
			}
		}
		objectToSnap = null;
		return default(RaycastHit);
	}
}
