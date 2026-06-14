using UnityEngine;

public class PlayerView : MonoBehaviour
{
	public FPSCamera FPScamera;

	public GameObject[] PlayerObjects;

	private CharacterSystem character;

	private void Start()
	{
		SetActive();
	}

	private void Awake()
	{
		character = GetComponent<CharacterSystem>();
		FPScamera = GetComponentInChildren<FPSCamera>();
	}

	private void Update()
	{
		SetActive();
	}

	public void SetActive()
	{
		if ((bool)character && character.IsMine)
		{
			GameObject[] playerObjects = PlayerObjects;
			GameObject[] array = playerObjects;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
			FPScamera.gameObject.SetActive(true);
		}
		else
		{
			FPScamera.gameObject.SetActive(false);
			GameObject[] playerObjects2 = PlayerObjects;
			GameObject[] array2 = playerObjects2;
			foreach (GameObject gameObject2 in array2)
			{
				gameObject2.SetActive(true);
			}
		}
	}

	public void Hide(Transform trans, bool hide)
	{
		foreach (Transform tran in trans)
		{
			tran.gameObject.SetActive(hide);
		}
		trans.gameObject.SetActive(hide);
	}
}
