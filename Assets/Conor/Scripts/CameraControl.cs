using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    Camera m_camera;

    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    void Update()
    {
        float moveSpeed;
        float camSpeed;

        if (Keyboard.current[Key.LeftShift].isPressed || Keyboard.current[Key.RightShift].isPressed)
        {
            moveSpeed = .2f;
            camSpeed = 3.0f;
        }
        else
        {
            moveSpeed = .1f;
            camSpeed = 1.5f;
        }

        Vector3 camPos = m_camera.transform.position;
        camPos.y -= Mouse.current.scroll.ReadValue().y * camSpeed;
        

        if(Keyboard.current[Key.W].isPressed || Keyboard.current[Key.UpArrow].isPressed) { camPos.z += moveSpeed;  }

        if (Keyboard.current[Key.A].isPressed || Keyboard.current[Key.LeftArrow].isPressed) { camPos.x -= moveSpeed; }

        if (Keyboard.current[Key.S].isPressed || Keyboard.current[Key.DownArrow].isPressed) { camPos.z -= moveSpeed;  }

        if(Keyboard.current[Key.D].isPressed || Keyboard.current[Key.RightArrow].isPressed) { camPos.x += moveSpeed;  }

        m_camera.transform.position = camPos;
    }
}
