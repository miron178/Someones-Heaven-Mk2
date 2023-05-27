using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : MonoBehaviour
{
    [SerializeField]
    Camera useCamera;

    Vector3 direction = Vector3.forward;

    Vector3 oldMousePos = Vector3.zero;

    void Update()
    {
        LookAtMouse();
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void LookAtMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        if (oldMousePos != mousePos)
        {
            oldMousePos = mousePos;

            Ray ray = useCamera.ScreenPointToRay(mousePos);
            Plane plane = new Plane(Vector3.up, transform.position);
            plane.Raycast(ray, out float distance);
            Vector3 target = ray.GetPoint(distance);

            direction = target - transform.position;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 pad = context.ReadValue<Vector2>();
        direction = new Vector3(pad.x, 0, pad.y);
    }
}