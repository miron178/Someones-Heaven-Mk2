using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetween : MonoBehaviour
{
    [SerializeField]
    private Transform[] destinations;
    private int dest = 0;

    [SerializeField]
    private float speed = 2;

    void FixedUpdate()
    {
        SelectNewDest();
        MoveToDest();
    }

    private void MoveToDest()
    {
        //move towards the current target
        Vector3 direction = destinations[dest].position - gameObject.transform.position;
        Vector3 move = direction.normalized * speed * Time.deltaTime;
        if (move.sqrMagnitude > direction.sqrMagnitude)
        {
            move = direction;
        }
        gameObject.transform.position += move;
    }

    private void SelectNewDest()
    {
        if (this.gameObject.transform.position == destinations[dest].position)
        {
            dest++;
            if (dest == destinations.Length)
            {
                dest = 0;
            }
        }
    }
}
