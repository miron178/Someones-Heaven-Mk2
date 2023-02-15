using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float speed = 0.5f;

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
        transform.position = Vector3.MoveTowards(transform.position, closest.transform.position, Time.deltaTime * speed);
    }

    void FixedUpdate()
    {
        ClosestTarget();
        MoveToClosest();
    }

}
