using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItsATrap : MonoBehaviour
{
    [SerializeField]
    [Range(0, Mathf.Infinity)]
    private float trapDeadlySpeed = 0.1f;

    [SerializeField]
    private bool enemyPushedToKill = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private bool IsDeadly()
    {
        return (trapDeadlySpeed == 0) || 
               (rb && rb.velocity.magnitude > trapDeadlySpeed);
    }

    private bool IsEnemyPushed(Enemy enemy)
    {
        return !enemy.agent.enabled;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (!enemy || !IsDeadly())
        {
            return;
        }
        if (enemyPushedToKill && !IsEnemyPushed(enemy))
        {
            return;
        }

        Destroy(enemy.gameObject);
    }
}
