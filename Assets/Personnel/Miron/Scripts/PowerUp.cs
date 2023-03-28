using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Tooltip("Type 'Inifinity' for permanent boost")]
    [SerializeField]
    float invincibilityDuration = 0f;

    [SerializeField]
    float speedBoost = 0f;

    [Tooltip("Type 'Inifinity' for permanent boost")]
    [SerializeField]
    float speedBoostDuration = 0f;

    [SerializeField]
    int maxHealthBoost = 0;

    [Tooltip("Type 'Inifinity' for permanent boost")]
    [SerializeField]
    float maxHealthBoostDuration = 0f;

    [SerializeField]
    int healthBoost = 0;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player)
        {
            player.AddInvinciblity(invincibilityDuration);
            player.SpeedBoost(speedBoost, speedBoostDuration);
            player.MaxHealthBoost(maxHealthBoost, maxHealthBoostDuration);
            player.HealthBoost(healthBoost);

            GameObject.Destroy(gameObject);
        }
    }
}
