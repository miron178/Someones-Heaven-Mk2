using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    [SerializeField]
    Camera useCamera;
    void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = useCamera.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Vector3.up, transform.position);
        plane.Raycast(ray, out float distance);
        Vector3 target = ray.GetPoint(distance);

        //Quaternion lookAt = Quaternion.LookRotation(target - transform.position);
        //transform.rotation = lookAt;

        transform.LookAt(target);
    }
}