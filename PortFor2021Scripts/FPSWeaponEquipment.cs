using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FPSWeaponEquipment : FPSItemEquipment
{
	private CharacterSystem character;

	private FPSController fpsController;

	private float timeTemp;

	private AudioSource audioSource;

	private Animator animator;

	public bool HoldFire = true;

	public float FireRate = 0.09f;

	public float Spread = 20f;

	public int BulletNum = 1;

	public int Damage = 10;

	public float Force = 10f;

	public Vector2 KickPower = Vector2.zero;

	public int MaxPenetrate = 1;

	public float Distance = 100f;

	public int Ammo = 30;

	public int AmmoMax = 30;

	public int ClipSize = 30;

	public int AmmoHave;

	public AudioClip SoundFire;

	public AudioClip SoundReload;

	public AudioClip SoundReloadComplete;

	public AudioClip[] DamageSound;

	public GameObject MuzzleFX;

	public Transform MuzzlePoint;

	public ItemData ItemUsed;

	public int UsingType;

	public bool InfinityAmmo;

	public bool OnAnimationEvent;

	public float AnimationSpeed = 1f;

	private float animationSpeedTemp = 1f;

	public float panicfire;

	public float PanicFireMult = 0.1f;

	public float FOVZoom = 65f;

	public float SpreadZoomMult = 1f;

	public bool HideWhenZoom;

	public GameObject ProjectileFX;

	public GUISkin Skin;

	private bool reloading;

	private float spreadmult;

	private void Start()
	{
		reloading = false;
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
		if ((bool)character)
		{
			character.DamageSound = DamageSound;
		}
		if ((bool)fpsController)
		{
			fpsController.zooming = false;
		}
		Hide(true);
		timeTemp = Time.time;
		StyleManager styleManager = (StyleManager)Object.FindObjectOfType(typeof(StyleManager));
		if ((bool)styleManager && !Skin)
		{
			Skin = styleManager.GetSkin(0);
		}
		if ((bool)animator)
		{
			animationSpeedTemp = animator.speed;
		}
	}

	public override void Trigger()
	{
		if (HoldFire || !OnFire1)
		{
			if ((bool)character && (bool)fpsController && Time.time > timeTemp + FireRate)
			{
				Shoot();
				timeTemp = Time.time;
			}
			base.Trigger();
		}
	}

	public override void OnTriggerRelease()
	{
		base.OnTriggerRelease();
	}

	public override void Trigger2()
	{
		fpsController.Zoom(FOVZoom);
		base.Trigger2();
	}

	public override void OnTrigger2Release()
	{
		base.OnTrigger2Release();
	}

	public void Shoot()
	{
		if ((bool)animator)
		{
			animator.speed = AnimationSpeed;
		}
		if (Ammo > 0 || InfinityAmmo)
		{
			if (!OnAnimationEvent)
			{
				OnAction();
			}
			if (character != null)
			{
				character.AttackAnimation(UsingType);
			}
			if ((bool)animator)
			{
				animator.SetTrigger("shoot");
			}
		}
	}

	public override void Reload()
	{
		if (Ammo >= AmmoMax || (Ammo <= 0 && AmmoHave <= 0))
		{
			return;
		}
		if (!reloading && (bool)audioSource && (bool)SoundReload)
		{
			audioSource.PlayOneShot(SoundReload);
		}
		if (ItemUsed != null)
		{
			int itemNum = character.inventory.GetItemNum(ItemUsed);
			if (!InfinityAmmo && character != null && character.inventory != null && itemNum <= 0)
			{
				return;
			}
		}
		if ((bool)animator)
		{
			animator.SetTrigger("reloading");
		}
		reloading = true;
		base.Reload();
	}

	public override void ReloadComplete()
	{
		int num = ClipSize;
		if (AmmoMax - Ammo < num)
		{
			num = AmmoMax - Ammo;
		}
		if (character != null && character.inventory != null && ItemUsed != null)
		{
			if (AmmoHave <= 0)
			{
				reloading = false;
				return;
			}
			if (AmmoHave < num)
			{
				num = AmmoHave;
			}
			character.inventory.RemoveItem(ItemUsed, num);
		}
		Ammo += num;
		if (Ammo >= AmmoMax)
		{
			reloading = false;
		}
		else
		{
			Reload();
		}
		if ((bool)audioSource && (bool)SoundReloadComplete)
		{
			audioSource.PlayOneShot(SoundReloadComplete);
		}
		base.ReloadComplete();
	}

	private void Update()
	{
		if (!reloading && Ammo <= 0)
		{
			Reload();
		}
		Hide(true);
		spreadmult = 1f;
		if (HideWhenZoom && (bool)fpsController && fpsController.zooming)
		{
			Hide(false);
			spreadmult = SpreadZoomMult;
		}
		if (panicfire < 0.01f)
		{
			panicfire = 0f;
		}
		panicfire += (0f - panicfire) * 5f * Time.deltaTime;
		if ((bool)animator)
		{
			animator.SetInteger("shoot_type", UsingType);
		}
		if (character != null && character.inventory != null && ItemUsed != null)
		{
			AmmoHave = character.inventory.GetItemNum(ItemUsed);
		}
		if (CollectorSlot != null)
		{
			CollectorSlot.NumTag = Ammo;
		}
	}

	public void SetCollectorSlot(ItemCollector collector)
	{
		CollectorSlot = collector;
		if (collector.NumTag != -1)
		{
			Ammo = collector.NumTag;
		}
	}

	private void OnGUI()
	{
		if (!InfinityAmmo)
		{
			if ((bool)Skin)
			{
				GUI.skin = Skin;
			}
			GUI.skin.label.alignment = TextAnchor.LowerRight;
			GUI.skin.label.fontSize = 35;
			GUI.Label(new Rect(Screen.width - 230, Screen.height - 90, 200f, 60f), Ammo + "/" + AmmoHave);
		}
	}

	public override void OnAction()
	{
		if ((bool)animator)
		{
			animator.speed = animationSpeedTemp;
		}
		if (Ammo > 0 || InfinityAmmo)
		{
			if (!InfinityAmmo)
			{
				Ammo--;
			}
			if ((bool)audioSource && (bool)SoundFire && audioSource.enabled)
			{
				audioSource.PlayOneShot(SoundFire);
			}
			if (BulletNum <= 0)
			{
				BulletNum = 1;
			}
			Vector3[] array = new Vector3[BulletNum];
			if (!MuzzlePoint)
			{
				MuzzlePoint = base.transform;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array.Length <= 1)
				{
					panicfire = 1f;
				}
				if ((bool)fpsController)
				{
					array[i] = fpsController.FPSCamera.transform.forward + new Vector3(Random.Range(0f - Spread + (float)(int)panicfire, Spread + (float)(int)panicfire) * 0.001f, Random.Range(0f - Spread + (float)(int)panicfire, Spread + (float)(int)panicfire) * 0.001f, Random.Range(0f - Spread + (float)(int)panicfire, Spread + (float)(int)panicfire) * 0.001f) * spreadmult;
				}
				if ((bool)ProjectileFX && (bool)fpsController)
				{
					{
						Object.Instantiate(ProjectileFX, fpsController.FPSCamera.transform.position + array[i] * 5f, Quaternion.LookRotation(array[i]));
					}
				}
				array[i] *= Force;
			}
			if ((bool)fpsController)
			{
				fpsController.Kick(KickPower);
			}
			if (character != null)
			{
				character.DoDamage(fpsController.FPSCamera.transform.position, array, Damage, Distance, MaxPenetrate, character.ID, character.Team);
			}
			if ((bool)MuzzleFX)
			{
				GameObject gameObject = Object.Instantiate(MuzzleFX, MuzzlePoint.position, MuzzlePoint.rotation);
				gameObject.transform.parent = base.transform;
				Object.Destroy(gameObject, 2f);
			}
			panicfire += PanicFireMult;
		}
		base.OnAction();
	}
}
