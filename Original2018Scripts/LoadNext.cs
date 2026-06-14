using UnityEngine;

public class LoadNext : MonoBehaviour
{
	public void Clicked()
	{
		Application.LoadLevel(0);
		Debug.Log("Main");
	}
}
