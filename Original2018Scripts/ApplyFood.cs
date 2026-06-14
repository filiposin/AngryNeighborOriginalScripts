using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class ApplyFood : MonoBehaviour
{
	public int FoodPlus = 10;

	public int DrinkPlus = 10;

	public int HealthPlus = 10;

	private void Start()
	{
		CharacterSystem characterSystem = (base.transform.root ? base.transform.root.GetComponent<CharacterSystem>() : base.transform.GetComponent<CharacterSystem>());
		if ((bool)characterSystem)
		{
			CharacterLiving component = characterSystem.GetComponent<CharacterLiving>();
			if ((bool)component)
			{
				component.Eating(FoodPlus);
				component.Drinking(DrinkPlus);
				component.Healing(HealthPlus);
			}
		}
		if (Network.isClient || Network.isServer)
		{
			Network.Destroy(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
