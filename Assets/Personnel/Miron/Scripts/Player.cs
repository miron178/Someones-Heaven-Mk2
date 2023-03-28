using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

	[SerializeField]
	private Animator Animator;

    [SerializeField]
    private float speed = 5f;
    private float speedBoost = 0f;
    private float speedBoostEnd = 0f;

    public float Speed
    {
        get
        {
            UpdateBoostFloat(ref speedBoost, ref speedBoostEnd);
            return speed + speedBoost;
        }
    }

    [SerializeField]
    private float pushForce = 300f;
    public float PushForce { get => pushForce; set => pushForce = value; }

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
	[SerializeField]
	private bool useGravityOnRoll = false;

    [SerializeField]
    private int maxHealth = 9;
    private int maxHealthBoost = 0;
    private float maxHealthBoostEnd = 0;

    public int MaxHealth
    {
        get
        {
            if (Time.time > maxHealthBoostEnd)
            {
                maxHealthBoost = 0;
            }
            return maxHealth + maxHealthBoost;
        }
    }

    [SerializeField]
    private int health = 9;

    public int Health
    {
        get
        {
            health = Mathf.Min(health, MaxHealth);
            return health;
        }
    }

    [SerializeField]
	private bool isDead = false;
	
	private Vector3 moveVelocity;
	private Vector3 rollVelocity;
    private float rollStart;
    private float rollEnd;

    [SerializeField]
    Material material;

    [SerializeField]
    HealthBar healthBar;

    [SerializeField] [Range (0,1)]
    float smoothRotation = 0.1f;

    private Vector3 velocity;

    private Sensor pushSensor;

    [SerializeField]
    GameObject model;

    private enum State
    {
		IDLE,
		WALKING,
        FALLING,
        ROLLING,
		DEAD,
    }
    private State state = State.FALLING;

    private void Start()
    {
		Animator = GetComponent<Animator>();
        pushSensor = GetComponentInChildren<Sensor>();
        CharacterController controller = GetComponent<CharacterController>();
        // calculate the correct vertical position:
        float correctHeight = controller.center.y + controller.skinWidth;
        // set the controller center vector:
        controller.center = new Vector3(0, correctHeight, 0);

        if (healthBar)
        {
            healthBar.UpdateHealth(this);
        }
    }

    void FixedUpdate()
    {

		if (Animator)
			Animator.SetBool("IsJumping", false);
		if (Animator)
			Animator.SetBool("TakeDamage", false);
		switch (state)
        {
			case State.IDLE:
				Idle();
                if(Animator)
				    Animator.SetBool("IsMoving", false);
				break;
            case State.WALKING:
                Walk();
                if(Animator)
				    Animator.SetBool("IsMoving", true);
				break;
            case State.FALLING:
                Fall();
                break;
            case State.ROLLING:
				if (Animator)
					Animator.SetBool("IsJumping", true);
				Roll();
				


				break;
			case State.DEAD:
				if (Animator)
					Animator.SetBool("IsDead", true);
				Dead();
				break;
			default:
                Debug.LogError("The state is unknown.");
                break;
        }

		//if (Keyboard.current[Key.Escape].wasReleasedThisFrame) {
		//	
		//}

		if (healthBar)
        {
            healthBar.UpdateHealth(this);
        }

        //cayan when IsInvincible
        //material.color = IsInvincible() ? Color.cyan : (CanRoll() ? Color.green : Color.yellow);

        //transparant when IsInvincible
        Color color = CanRoll() ? Color.green : Color.yellow;
        color.a = IsInvincible() ? 0.5f : 1f;
        material.color = color;
    }

	void StartIdle()
	{
		state = State.IDLE;
		
	}

	void Idle()
	{
		if (moveVelocity.sqrMagnitude > 0)
		{
			StartWalk();
		}
		if (isDead == true)
		{
			Die();
		}
	}

    public void TakeDamage(int damage)
    {
		
		if (!IsInvincible())
        {
            health -= damage < health ? damage : health;
			Animator.SetBool("TakeDamage", true);
			if (Animator)
				Animator.SetBool("TakeDamage", true);
		}
     
        if (health == 0)
        {
            Die();
        }
		
	}

    void Die()
	{
		state = State.DEAD;
	}

	void Dead()
	{
		//TODO: do things on reset
		//Enjoy being dead
	}
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        Vector2 moveAmount = context.ReadValue<Vector2>();
        moveVelocity = (transform.right * moveAmount.x + transform.forward * moveAmount.y);
    }

    public void OnPush(InputAction.CallbackContext context)
    {
        if (context.performed && state != State.DEAD)
        {
            foreach (GameObject target in pushSensor.InSight(pushSensor.layers))
            {
                Enemy tempEnemy = target.GetComponent<Enemy>();
                Vector3 dir = tempEnemy.transform.position - transform.position;
                tempEnemy.Push(dir.normalized * pushForce);
            }
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded && CanRoll() && state != State.DEAD)
        {
            StartRoll();
			
		}
    }

    private void StartWalk()
    {
        state = State.WALKING;
    }

    private void Walk()
    {
        velocity = moveVelocity * Speed;
		velocity.y = gravity;

        Vector3 movement = velocity * Time.deltaTime;

		//ignore Gravity for minMoveDistance check
		Vector3 movementXZ = movement;
		movementXZ.y = 0;

        if (model != null)
        {
            Quaternion lookAt = Quaternion.LookRotation(movementXZ);
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, lookAt, smoothRotation);
        }

        if (movementXZ.magnitude >= controller.minMoveDistance)
        {
            controller.Move(movement);
            if (!controller.isGrounded)
            {
                StartFall();
            }
        }
		else
		{
			StartIdle();
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
            StartIdle();
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
		rollVelocity.y = useGravityOnRoll ? gravity : 0;
        rollEnd = Time.time + rollDuration;
        rollStart = rollEnd + rollCD;

        state = State.ROLLING;

        AddInvinciblity(invincibiltyDuration);

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

        if (Time.time >= rollEnd || (useGravityOnRoll && !controller.isGrounded))
        {
			
			StartFall();
			
		}
		
    }

    bool IsInvincible()
    {
		return Time.time < invincibiltyEnd;
		
	}

    public void AddInvinciblity(float duration)
    {
        float start = Mathf.Max(invincibiltyEnd, Time.time);
        invincibiltyEnd = start + duration;
    }

    private void UpdateBoostFloat(ref float paramBoost, ref float paramBoostEnd)
    {
        if (Time.time > paramBoostEnd)
        {
            paramBoost = 0;
        }
    }

    public void SpeedBoost(float boost, float duration)
    {
        if (duration == Mathf.Infinity)
        {
            speed += boost;
        }
        else
        {
            speedBoost += boost;
            speedBoostEnd = Mathf.Max(Time.time, speedBoostEnd) + duration;
        }
    }

    public void HealthBoost(int boost)
    {
        health = Mathf.Min(health + boost, MaxHealth);
    }

    public void MaxHealthBoost(int boost, float duration)
    {
        if (duration == Mathf.Infinity)
        {
            maxHealth += boost;
        }
        else
        {
            maxHealthBoost += boost;
            maxHealthBoostEnd = Mathf.Max(Time.time, maxHealthBoostEnd) + duration;
        }
    }
}
