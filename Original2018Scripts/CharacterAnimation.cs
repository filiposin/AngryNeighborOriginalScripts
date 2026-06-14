using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	private Animator animator;

	public Transform upperSpine;

	public Transform headCamera;

	public Quaternion CameraRotation;

	private CharacterSystem character;

	private void Start()
	{
		animator = GetComponent<Animator>();
		character = GetComponent<CharacterSystem>();
		if (headCamera == null)
		{
			FPSCamera componentInChildren = GetComponentInChildren<FPSCamera>();
			headCamera = componentInChildren.gameObject.transform;
		}
	}

	private void Update()
	{
		if (animator == null || character == null || !upperSpine)
		{
			return;
		}
		if (character.IsMine)
		{
			CameraRotation = upperSpine.localRotation;
			CameraRotation.eulerAngles = new Vector3(upperSpine.localRotation.eulerAngles.x, upperSpine.localRotation.eulerAngles.y, 0f - headCamera.transform.rotation.eulerAngles.x);
			if (Network.isServer || Network.isClient)
			{
				GetComponent<NetworkView>().RPC("UpdateHeadRotation", RPCMode.Others, CameraRotation);
			}
		}
		upperSpine.transform.localRotation = CameraRotation;
		if ((bool)animator.GetComponent<Animation>() && (bool)animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name])
		{
			animator.GetComponent<Animation>()[animator.GetComponent<Animation>().clip.name].AddMixingTransform(upperSpine);
		}
	}

	[RPC]
	private void UpdateHeadRotation(Quaternion rotation)
	{
		CameraRotation = rotation;
	}
}
