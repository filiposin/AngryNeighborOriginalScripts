using UnityEngine;

[RequireComponent(typeof(CharacterSystem))]
public class CharacterLiving : MonoBehaviour
{
	public int Hungry = 100;

	public int HungryMax = 100;

	public int Water = 100;

	public int WaterMax = 100;

	[HideInInspector]
	public CharacterSystem character;

	[HideInInspector]
	public NetworkView networkViewer;

	private void Start()
	{
		networkViewer = GetComponent<NetworkView>();
		character = GetComponent<CharacterSystem>();
		InvokeRepeating("stomachUpdate", 1f, 1f);
		InvokeRepeating("hungryUpdate", 1f, 15f);
		InvokeRepeating("thirstilyUpdate", 1f, 10f);
	}

	private void Update()
	{
		if (Hungry < 0)
		{
			Hungry = 0;
		}
		if (Hungry > HungryMax)
		{
			Hungry = HungryMax;
		}
		if (Water < 0)
		{
			Water = 0;
		}
		if (Water > WaterMax)
		{
			Water = WaterMax;
		}
	}

	public void stomachUpdate()
	{
		if (character == null)
		{
			return;
		}
		if (Water <= 0)
		{
			if (Network.isServer && !Network.isClient)
			{
				if ((bool)networkViewer)
				{
					networkViewer.RPC("applyDamage", RPCMode.All, 2, Vector3.up, character.ID, string.Empty);
				}
			}
			else
			{
				character.ApplyDamage(2, Vector3.up, character.ID, string.Empty);
			}
		}
		if (Hungry > 0)
		{
			return;
		}
		if (Network.isServer && !Network.isClient)
		{
			if ((bool)networkViewer)
			{
				networkViewer.RPC("applyDamage", RPCMode.All, 2, Vector3.up, character.ID, string.Empty);
			}
		}
		else
		{
			character.ApplyDamage(1, Vector3.up, character.ID, string.Empty);
		}
	}

	public void hungryUpdate()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer))
		{
			Eating(-1);
		}
	}

	public void thirstilyUpdate()
	{
		if (Network.isServer || (!Network.isClient && !Network.isServer))
		{
			Drinking(-1);
		}
	}

	[RPC]
	private void eatUpdate(int num)
	{
		Hungry = num;
	}

	[RPC]
	public void Eating(int num)
	{
		if (!Network.isServer && !Network.isClient)
		{
			Hungry += num;
			return;
		}
		if (Network.isClient && (bool)networkViewer)
		{
			networkViewer.RPC("Eating", RPCMode.Server, num);
		}
		if (Network.isServer)
		{
			Hungry += num;
			if ((bool)networkViewer)
			{
				networkViewer.RPC("eatUpdate", RPCMode.Others, Hungry);
			}
		}
	}

	[RPC]
	private void drinkUpdate(int num)
	{
		Water = num;
	}

	[RPC]
	public void Drinking(int num)
	{
		if (!Network.isServer && !Network.isClient)
		{
			Water += num;
			return;
		}
		if (Network.isClient && (bool)networkViewer)
		{
			networkViewer.RPC("Drinking", RPCMode.Server, num);
		}
		if (Network.isServer)
		{
			Water += num;
			if ((bool)networkViewer)
			{
				networkViewer.RPC("drinkUpdate", RPCMode.Others, Water);
			}
		}
	}

	[RPC]
	private void healthUpdate(int num)
	{
		if (!(character == null))
		{
			character.HP += num;
		}
	}

	[RPC]
	public void Healing(int num)
	{
		if (character == null)
		{
			return;
		}
		if (!Network.isServer && !Network.isClient)
		{
			character.HP += num;
			return;
		}
		if (Network.isClient && (bool)networkViewer)
		{
			networkViewer.RPC("healthUpdate", RPCMode.Server, num);
		}
		if (Network.isServer)
		{
			Hungry += num;
			if ((bool)networkViewer)
			{
				networkViewer.RPC("healthUpdate", RPCMode.Others, num);
			}
		}
	}
}
