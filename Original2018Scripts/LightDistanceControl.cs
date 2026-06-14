// Ваша структура
using UnityEngine;

public class LightDistanceControl : MonoBehaviour
{
    private Light lightSource;
    private Transform player;
    public float maxDistance = 10f;

    void Update()
    {
        player = GameObject.Find("Character_Salary(Clone)").GetComponent<Transform>();
        // Получаем компонент Light
        lightSource = GetComponent<Light>();
        if (player != null)
        {
            float distance = Vector3.Distance(lightSource.transform.position, player.position);
            lightSource.enabled = (distance <= maxDistance);
        }
    }
}
