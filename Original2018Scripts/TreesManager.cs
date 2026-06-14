using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{
	public int TreeHP = 200;

	public int DropNum = 10;

	public float DropArea = 3f;

	public Terrain terrain;

	public float BlastArea = 0.5f;

	public GameObject WoodDrop;

	public float ResetTime = 600f;

	private List<int> RemovedIndexList = new List<int>();

	private SceneManager sceneManage;

	private List<Logger> Loggers = new List<Logger>();

	private TreeInstance[] treesBackup;

	private int ChopIndex;

	private bool hited;

	private float timeTemp;

	private void Start()
	{
		sceneManage = (SceneManager)Object.FindObjectOfType(typeof(SceneManager));
		if ((bool)sceneManage && Network.isClient)
		{
			sceneManage.GetInitializeeScene();
		}
		if (terrain == null)
		{
			terrain = GetComponent<Terrain>();
		}
		timeTemp = Time.time;
		BackupTree();
	}

	private void Update()
	{
		hited = false;
		if (!(Time.time >= ResetTime + timeTemp))
		{
			return;
		}
		if (Network.isServer || Network.isClient)
		{
			if (Network.isServer && (bool)sceneManage)
			{
				sceneManage.ResetAllTrees();
			}
		}
		else
		{
			ResetTrees();
		}
		timeTemp = Time.time;
	}

	public void ResetTrees()
	{
		RemovedIndexList.Clear();
		Loggers.Clear();
		terrain.terrainData.treeInstances = treesBackup;
	}

	private bool LoggerChecker(int index)
	{
		foreach (Logger logger2 in Loggers)
		{
			if (logger2.index == index)
			{
				return false;
			}
		}
		Logger logger = new Logger();
		logger.index = index;
		logger.HP = TreeHP;
		Loggers.Add(logger);
		return true;
	}

	private bool LoggerApplyDamage(int damage, int index)
	{
		if (!LoggerChecker(index))
		{
			foreach (Logger logger in Loggers)
			{
				if (logger.index == index)
				{
					logger.HP -= damage;
					if (logger.HP <= 0)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void Cuttree(Vector3 position, int damage)
	{
		if (terrain == null)
		{
			return;
		}
		int num = 0;
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		for (int i = 0; i < treeInstances.Length; i++)
		{
			TreeInstance treeInstance = treeInstances[i];
			Vector3 vector = Vector3.Scale(treeInstance.position, terrain.terrainData.size) + Terrain.activeTerrain.transform.position;
			float num2 = Vector3.Distance(new Vector3(vector.x, position.y, vector.z), position);
			if (num2 < BlastArea && LoggerApplyDamage(damage, num))
			{
				Drop(position);
				RemovedIndexList.Add(num);
				SendARemovedTree(num);
				break;
			}
			num++;
		}
	}

	public void SendARemovedTree(int index)
	{
		if (!Network.isServer && !Network.isClient)
		{
			RemoveATrees(index);
		}
		else if (Network.isServer)
		{
			RemoveATrees(index);
			if ((bool)sceneManage)
			{
				sceneManage.SendRemovedTreeIndex(index);
			}
		}
	}

	public string GetRemovedTrees()
	{
		string text = string.Empty;
		foreach (int removedIndex in RemovedIndexList)
		{
			text = text + removedIndex + ",";
		}
		return text;
	}

	public void UpdateRemovedTrees(string indexremoved)
	{
		if (terrain == null)
		{
			return;
		}
		RemovedIndexList.Clear();
		string[] array = indexremoved.Split(","[0]);
		for (int i = 0; i < array.Length; i++)
		{
			int result = -1;
			if (int.TryParse(array[i], out result))
			{
				RemovedIndexList.Add(result);
			}
		}
		List<TreeInstance> list = new List<TreeInstance>(terrain.terrainData.treeInstances);
		foreach (int removedIndex in RemovedIndexList)
		{
			list.RemoveAt(removedIndex);
		}
		terrain.terrainData.treeInstances = list.ToArray();
	}

	public void RemoveATrees(int index)
	{
		List<TreeInstance> list = new List<TreeInstance>();
		int num = 0;
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		TreeInstance[] array = treeInstances;
		foreach (TreeInstance item in array)
		{
			if (num != index)
			{
				list.Add(item);
			}
			num++;
		}
		terrain.terrainData.treeInstances = list.ToArray();
	}

	public void OnHit(DamagePackage dm)
	{
		if (!hited && (Network.isServer || (!Network.isClient && !Network.isServer)))
		{
			Cuttree(dm.Position, dm.Damage);
			hited = true;
		}
	}

	public void Drop(Vector3 position)
	{
		if (WoodDrop == null)
		{
			return;
		}
		for (int i = 0; i < DropNum; i++)
		{
			if (Network.isClient || Network.isServer)
			{
				if (Network.isServer)
				{
					Network.Instantiate(WoodDrop, DetectGround(position + new Vector3(Random.Range(0f - DropArea, DropArea), 0f, Random.Range(0f - DropArea, DropArea))), WoodDrop.transform.rotation, 2);
				}
			}
			else
			{
				Object.Instantiate(WoodDrop, DetectGround(position + new Vector3(Random.Range(0f - DropArea, DropArea), 0f, Random.Range(0f - DropArea, DropArea))), WoodDrop.transform.rotation);
			}
		}
	}

	private Vector3 DetectGround(Vector3 position)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(position, -Vector3.up, out hitInfo, 1000f))
		{
			return hitInfo.point;
		}
		return position;
	}

	private void BackupTree()
	{
		if (!(terrain == null))
		{
			treesBackup = terrain.terrainData.treeInstances;
		}
	}

	private void OnApplicationQuit()
	{
		if (!(terrain == null))
		{
			terrain.terrainData.treeInstances = treesBackup;
		}
	}
}
