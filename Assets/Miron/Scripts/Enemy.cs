using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    private static string SelectedTag = "Player";

    GameObject[] targets;
    GameObject closest;

    GameObject ClosestTarget()
    {
        targets = GameObject.FindGameObjectsWithTag(SelectedTag);

        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject target in targets)
        {
            Vector3 difference = target.transform.position - pos;
            float currentDistance = difference.sqrMagnitude;
            if (currentDistance < distance)
            {
                closest = target;
                distance = currentDistance;
            }
        }
        return closest;
    }

    void MoveToClosest()
    {
        agent.SetDestination(closest.transform.position);
    }

    void FixedUpdate()
    {
        ClosestTarget();
        MoveToClosest();
    }

}
