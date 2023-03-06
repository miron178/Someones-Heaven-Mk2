using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    [SerializeField]
    private static string SelectedTag = "Player";
    [SerializeField]
    private float endPushSpeed = 0.1f;

    [SerializeField]
    float radiusToTarget = 1.25f;

    [SerializeField]
    private int attackDamage = 1;

    [SerializeField] [Range(0,1)]
    private float hitChance = 0.5f;

    [SerializeField]
    private float attackRange = 2;

    [SerializeField]
    private float attackCD = 1;
    private float attackTime = 0;

    [SerializeField]
    Material material;

    GameObject[] targets;
    GameObject closest;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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

    bool IsPushActive()
    {
        return !agent.enabled;
    }

    void StartPush()
    {
        agent.enabled = false;
        rb.isKinematic = false;
    }

    void EndPush()
    {
        agent.enabled = true;
        rb.isKinematic = true;
    }

    void MoveToClosest()
    {
        //radius round target
        Vector3 target = transform.position - closest.transform.position;
        target = target.normalized * radiusToTarget + closest.transform.position;

        //move to target radius
        agent.SetDestination(target);

        //Debug.DrawLine(target, transform.position);
    }

    void FixedUpdate()
    {
        if (IsPushActive())
        {
            Vector3 horiz = rb.velocity;
            horiz.y = 0;

            //This function can be called before rb.AddForce() actually changes the velocity
            //hence the > 0 check
            bool slowedDown = horiz.sqrMagnitude > 0 && horiz.sqrMagnitude < endPushSpeed * endPushSpeed;
            if (slowedDown)
            {
                EndPush();
            }
        }
        else if (CanAttack())
        {
            Attack();
        }
        else
        {
            ClosestTarget();
            MoveToClosest();
        }
        Color color = Time.time >= attackTime ? Color.red : Color.black;
        material.color = color;
    }

    private bool CanAttack()
    {
        if (!closest || Time.time < attackTime)
        {
            return false;
        }

        Vector3 distance = transform.position - closest.transform.position;
        return distance.magnitude <= attackRange;
    }

    private void Attack()
    {
        bool hit = Random.value <= hitChance;
        if (hit)
        {
            Player player = closest.GetComponent<Player>();
            player.TakeDamage(attackDamage);
        }
        attackTime = Time.time + attackCD;
    }

    public void Push(Vector3 force)
    {
        StartPush();
        rb.AddForce(force);
    }
}
