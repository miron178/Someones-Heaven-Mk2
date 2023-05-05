using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomPointGenerator : MonoBehaviour
{
    public static Vector3 PointGen (Vector3 StartPoint, float Radius)
    {
        Vector3 Dir = Random.insideUnitSphere * Radius;
        Dir += StartPoint;
        NavMeshHit Hit;
        Vector3 FinalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(Dir, out Hit, Radius, 1)){
            FinalPosition = Hit.position;
        }
        return FinalPosition;
    }
}
