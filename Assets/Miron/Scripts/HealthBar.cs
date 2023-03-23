using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private GameObject healthFull;

    [SerializeField]
    private GameObject healthEmpty;

    [SerializeField]
    private float offset = 15;

    [SerializeField]
    private float distance = 30;

    private int currentHealth = 0;
    private int maxHealth = 0;

    private List<GameObject> health;

    private void Start()
    {
    }

    private bool HealthChange(Player player)
    {
        bool changed = false;
        if (currentHealth != player.Health)
        {
            currentHealth = player.Health;
            changed = true;
        }
        if (maxHealth != player.MaxHealth)
        {
            maxHealth = player.MaxHealth;
            changed = true;
        }
        return changed;
    }
    private void ClearHealth()
    {
        if (health == null)
        {
            health = new List<GameObject>();
        }
        foreach (GameObject obj in health)
        {
            GameObject.Destroy(obj);
        }
        health.Clear();
    }

    public void UpdateHealth(Player player)
    {
        if (HealthChange(player))
        {
            ClearHealth();
            for (int i = 0; i < player.MaxHealth; i++)
            {
                bool full = i < player.Health;
                GameObject obj = GameObject.Instantiate(full ? healthFull : healthEmpty, transform);
                obj.transform.localPosition = new Vector3(offset + i * distance, 0, 0);
                health.Add(obj);
            }
        }
    }
}
