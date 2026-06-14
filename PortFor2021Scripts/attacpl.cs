using UnityEngine;

public class attacpl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character_Salary(Clone)")
        {
            Object.Destroy(other.gameObject);
        }
    }
}