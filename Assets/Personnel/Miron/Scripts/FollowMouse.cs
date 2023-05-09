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
        LookAtMouse();
    }

    void LookAtMouse()
    {
        //redo for controler (second function?)
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = useCamera.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Vector3.up, transform.position);
        plane.Raycast(ray, out float distance);
        Vector3 target = ray.GetPoint(distance);

        transform.LookAt(target);
    }
}