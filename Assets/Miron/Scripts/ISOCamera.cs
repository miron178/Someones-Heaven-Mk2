using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISOCamera : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 offset;

    [Range(0.1f, 10f)]
    [SerializeField]
    float smoothSpeed = 1f;

    [SerializeField]
    bool cameraDetermainsFront = false;

    void LateUpdate()
    {
        //Smooth follow
        Vector3 desiredPostion = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPostion, smoothSpeed * Time.deltaTime);

        //Smooth LookAt()
        Quaternion current = transform.rotation;
        transform.LookAt(target.position);
        Quaternion desired = transform.rotation;
        transform.rotation = Quaternion.Lerp(current, desired, smoothSpeed * Time.deltaTime);

        if(cameraDetermainsFront)
        {
            Vector3 level = transform.position;
            level.y = target.position.y; //camera lowered to player level
            Vector3 away = target.position - level;
            away.y = target.position.y;
            target.forward = away.normalized;
        }
    }
}
