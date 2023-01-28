using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float speed = 5f;

    public float gravity = -9.81f;

    [SerializeField]
    float invincibiltyDuration = 0.5f;
    private float invincibiltyEnd = 0;

    [SerializeField]
    private float rollSpeed = 10;
    [SerializeField]
    private bool canRoll = true;
    [SerializeField]
    private float rollCD = 1;
    [SerializeField]
    private float rollDuration = 0.5f;

    private Vector3 rollVelocity;
    private float rollStart;
    private float rollEnd;

    [SerializeField]
    Material material;
    
    private Vector3 velocity;

    private enum State
    {
        WALKING,
        FALLING,
        ROLLING,
    }
    private State state = State.FALLING;

    private void Start()
    {
        CharacterController controller = GetComponent<CharacterController>();
        // calculate the correct vertical position:
        float correctHeight = controller.center.y + controller.skinWidth;
        // set the controller center vector:
        controller.center = new Vector3(0, correctHeight, 0);
    }

    void Update()
    {
        switch (state)
        {
            case State.WALKING:
                Walk();
                break;
            case State.FALLING:
                Fall();
                break;
            case State.ROLLING:
                Roll();
                break;
            default:
                Debug.LogError("The state is unknown.");
                break;
        }

        //cayan when IsInvincible
        //material.color = IsInvincible() ? Color.cyan : (CanRoll() ? Color.green : Color.yellow);

        //transparant when IsInvincible
        Color color = CanRoll() ? Color.green : Color.yellow;
        color.a = IsInvincible() ? 0.5f : 1f;
        material.color = color;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        Vector2 moveAmount = context.ReadValue<Vector2>();

        //walk
        Vector3 move = (transform.right * moveAmount.x + transform.forward * moveAmount.y) * speed;
        velocity.x = move.x;
        velocity.z = move.z;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded && CanRoll())
        {
            StartRoll();
        }
    }

    private void StartWalk()
    {
        state = State.WALKING;
        velocity.y = 0;
    }

    private void Walk()
    {
        velocity.y = gravity;

        Vector3 movement = velocity * Time.deltaTime;
        if (movement.magnitude >= controller.minMoveDistance)
        {
            controller.Move(movement);
            if (!controller.isGrounded)
            {
                StartFall();
            }
        }
    }

    private void StartFall()
    {
        state = State.FALLING;
    }

    private void Fall()
    {
        //*-1 to make value positive (gravity is -9.81)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            StartWalk();
        }
    }

    private bool CanRoll()
    {
        return canRoll && Time.time >= rollStart;
    }

    private void StartRoll()
    {
        Vector3 horizVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (horizVelocity.sqrMagnitude > 0)
        {
            rollVelocity = horizVelocity.normalized * rollSpeed;
        }
        else
        {
            rollVelocity = transform.forward * rollSpeed;
        }
        rollVelocity.y = gravity;
        rollEnd = Time.time + rollDuration;
        rollStart = rollEnd + rollCD;

        state = State.ROLLING;

        invincibiltyEnd = Time.time + invincibiltyDuration;
    }

    private void Roll()
    {
        if (Time.time < rollEnd)
        {
            velocity = rollVelocity;
            Vector3 movement = velocity * Time.deltaTime;
            controller.Move(movement);
        }
        else
        {
            velocity = Vector3.zero;
        }

        if (Time.time >= rollEnd || !controller.isGrounded)
        {
            StartFall();
        }
    }

    bool IsInvincible()
    {
        return Time.time < invincibiltyEnd;
    }
}
