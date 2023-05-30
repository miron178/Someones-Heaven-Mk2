using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private CharacterController controller;

	[SerializeField]
	private Animator Animator;

	public PowerUpCore playerPowerUp;

	public ParticleHolder particleHolder;

    [SerializeField]
    public float speed = 5f;
	public float originalSpeed;
	[SerializeField]
	public float runSpeed = 10f;
	public float originalRunspeed;
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
    private float pushForceMax = 300f;
    public float PushForceMax { get => pushForceMax; }

	[SerializeField]
	private float pushRamp = 1f;

	private float pushForceCurrent = 0f;
	public float PushForceCurrent { get => pushForceCurrent; }
	private bool pushHold = false;

	[SerializeField]
	private float throwForceMax = 300f;
	public float ThrowForceMax { get => throwForceMax; }

	[SerializeField]
	private float throwRamp = 1f;

	private float throwForceCurrent = 0f;
	public float ThrowForceCurrent { get => throwForceCurrent; }
	private bool throwHold = false;

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
	private float jumpSpeed = 10;
	[SerializeField]
	private bool canJump = true;
	[SerializeField]
	private float JumpCD = 1;
	[SerializeField]
	private float jumpDuration = 0.5f;
	[SerializeField]
	private bool useGravityOnJump = false;

	[SerializeField]
    private int maxHealth = 9;
    private int maxHealthBoost = 0;
    private float maxHealthBoostEnd = 0;

    private Vector3 pull = Vector3.zero;

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
    public int health = 9;
	public bool canDamage;

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
	private Vector3 jumpVelocity;
	private float jumpStart;
	private float jumpEnd;

	[SerializeField]
    Material material;

    [SerializeField]
    HealthBar healthBar;

    [SerializeField] [Range (0,1)]
    float smoothRotation = 0.1f;

    private Vector3 velocity;

    private Sensor pushSensor;

	public float animationSwitchTime = 0.2f;

    [SerializeField]
    GameObject model;

	[SerializeField]
	Vector3 torchThrowVector;

	[SerializeField]
	Transform torchThrower;

	[SerializeField]
	GameObject torchPrefab;

	[SerializeField]
	GameObject torchThrowCharge;

	private bool throwing = false;


	private enum State
    {
		IDLE,
		WALKING,
		RUNNING,
        FALLING,
        ROLLING,
		JUMPING,
		DEAD,
    }
    private State state = State.FALLING;

    private void Start()
    {
		canDamage = true;
		particleHolder = gameObject.GetComponent<ParticleHolder>();
		originalSpeed = speed;
		originalRunspeed = runSpeed;
		Animator = GetComponent<Animator>();
        pushSensor = GetComponentInChildren<Sensor>();
        CharacterController controller = GetComponent<CharacterController>();
        // calculate the correct vertical position:
        float correctHeight = controller.center.y + controller.skinWidth;
        // set the controller center vector:
        controller.center = new Vector3(0, correctHeight, 0);
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();

        if (healthBar)
        {
            healthBar.UpdateHealth(this);
        }
    }

    void FixedUpdate()
    {

		if (Keyboard.current.leftShiftKey.isPressed) {
			state = State.RUNNING;
		}
		if (Keyboard.current.leftShiftKey.wasReleasedThisFrame) {
			state = State.WALKING;
		}
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
			case State.RUNNING:
				Run();
				if (Animator)
					Animator.SetBool("IsRunning", true);
				break;
			case State.FALLING:
                Fall();
                break;
            case State.ROLLING:
				Roll();
				if (Animator)
					Animator.SetBool("IsRolling", true);
				break;
			case State.JUMPING:
				if (Animator)
					Animator.SetBool("IsJumping", true);
				Jump();
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

        // All pull forces are applied now, reset before the next update
        pull = Vector3.zero;

		//if (Keyboard.current[Key.Escape].wasReleasedThisFrame) {
		//	
		//}

		if (healthBar)
        {
            healthBar.UpdateHealth(this);
        }

		if (pushHold)
        {
			pushForceCurrent = Mathf.Min(pushForceCurrent + pushRamp * Time.deltaTime, pushForceMax);
		}

		if (throwHold)
		{
			throwForceCurrent = Mathf.Min(throwForceCurrent + throwRamp * Time.deltaTime, throwForceMax);
		}

		//cayan when IsInvincible
		//material.color = IsInvincible() ? Color.cyan : (CanRoll() ? Color.green : Color.yellow);

		//transparant when IsInvincible
		if (material != null)
        {
            Color color = CanRoll() ? Color.green : Color.yellow;
            color.a = IsInvincible() ? 0.5f : 1f;
            material.color = color;
        }

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
        else
        {
            controller.Move(pull * Time.deltaTime);
        }
		if (isDead == true)
		{
			Die();
		}
	}

    public void TakeDamage(int damage)
    {	
		if (!IsInvincible() && canDamage)
        {
            health -= damage < health ? damage : health;
			AddInvinciblity(invincibiltyDuration);
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
		Invoke("SwitchAnimationBools", animationSwitchTime);
	}

	public void OnRun(InputAction.CallbackContext context) {
		// read the value for the "move" action each event call
		StartRun();
		Vector2 moveAmount = context.ReadValue<Vector2>();
		moveVelocity = (transform.right * moveAmount.x + transform.forward * moveAmount.y);
		Invoke("SwitchAnimationBools", animationSwitchTime);
	}


	public void OnPush(float magnitude)
    {
		if (state == State.DEAD)
			return;

		foreach (GameObject target in pushSensor.InSight(pushSensor.layers))
		{
			Vector3 dir = target.transform.position - transform.position;
			Vector3 force = dir.normalized * magnitude;
			IPushable pushable = target.GetComponent<IPushable>();
			if (pushable != null)
			{
				pushable.Push(force);
			}
			else
			{
				Rigidbody rb = target.GetComponent<Rigidbody>();
				if (rb)
				{
					rb.AddForce(force);
				}
			}
		}
		Invoke("SwitchAnimationBools", animationSwitchTime);
	}

	public void OnThrowStart()
    {
		throwing = true;
	}
	public void OnThrowEnd(float magnitude)
	{
		Vector3 dir = torchThrowVector;
		Vector3 force = dir.normalized * magnitude;
		Rigidbody rb = torchPrefab.GetComponent<Rigidbody>();
		rb.AddForce(force);
		Invoke("SwitchAnimationBools", animationSwitchTime);
		torchThrowCharge.SetActive(false);
		throwing = false;
	}

	public void OnThrowCancel()
    {
		throwing = false;
	}

	public void OnTorch(InputAction.CallbackContext context)
	{
		if (context.canceled)
		{
			if (!torchPrefab.activeSelf)
			{
				torchThrowCharge.SetActive(true);
				torchPrefab.SetActive(true);
			}
			else if (!throwing)
			{
				torchThrowCharge.SetActive(false);
				torchPrefab.SetActive(false);
			}
		}
	}

	public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded && CanRoll() && state != State.DEAD)
        {
            StartRoll();
			Invoke("SwitchAnimationBools", animationSwitchTime);
		}
    }

	public void OnJump(InputAction.CallbackContext context) {
		if (context.performed && controller.isGrounded && CanJump() && state != State.DEAD) {
			StartJump();
			Invoke("SwitchAnimationBools", animationSwitchTime);
		}
	}

	private void StartWalk()
    {
        state = State.WALKING;
    }

    private void Walk()
    {
        velocity = moveVelocity * Speed + pull;
		velocity.y = gravity;

        Vector3 movement = velocity * Time.deltaTime;

		//ignore Gravity for minMoveDistance check
		Vector3 movementXZ = movement;
		movementXZ.y = 0;

        if (model != null && movementXZ != Vector3.zero)
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

	private void StartRun() {
		state = State.RUNNING;
	}

	private void Run() {
		velocity = moveVelocity * runSpeed + pull;
		velocity.y = gravity;

		Vector3 movement = velocity * Time.deltaTime;

		//ignore Gravity for minMoveDistance check
		Vector3 movementXZ = movement;
		movementXZ.y = 0;

		if (model != null) {
			Quaternion lookAt = Quaternion.LookRotation(movementXZ);
			model.transform.rotation = Quaternion.Lerp(model.transform.rotation, lookAt, smoothRotation);
		}

		if (movementXZ.magnitude >= controller.minMoveDistance) {
			controller.Move(movement);
			if (!controller.isGrounded) {
				StartFall();
			}
		} else {
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
            velocity = rollVelocity + pull;
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

	private bool CanJump() {
		return canJump && Time.time >= jumpStart;
	}

	private void StartJump() {

		Vector3 horizVelocity = new Vector3(velocity.x, 0, velocity.z);
		if (horizVelocity.sqrMagnitude > 0) {
			jumpVelocity = horizVelocity.normalized * jumpSpeed;
		} else {
			jumpVelocity = transform.forward * jumpSpeed;
		}
		jumpVelocity.y = useGravityOnJump ? gravity : 0;
		jumpEnd = Time.time + jumpDuration;
		jumpStart = jumpEnd + JumpCD;

		state = State.JUMPING;

		AddInvinciblity(invincibiltyDuration);

	}

	private void Jump() {

		if (Time.time < jumpEnd) {
			velocity = jumpVelocity + pull;
			Vector3 movement = velocity * Time.deltaTime;
			controller.Move(movement);
		} else {
			velocity = Vector3.zero;
		}

		if (Time.time >= jumpEnd || (useGravityOnJump && !controller.isGrounded)) {

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

    public void Push(Vector3 force)
    {
        pull += force;
    }

	public void OnTriggerEnter(Collider other) {
		
		if (other.CompareTag("Powerup")) {
			
			playerPowerUp = other.GetComponent<PowerUpCore>();
			particleHolder.timeout = playerPowerUp.effectTimeOut;
			playerPowerUp.PickUp();
			Invoke("ResetStats", particleHolder.timeout);
			if (playerPowerUp.isDefence) {
				particleHolder.DefenceBubbleOn();
				particleHolder.Invoke("DefenceBubbleOff", particleHolder.timeout);
			}
			if (playerPowerUp.isSpeed) {
				particleHolder.SpeedBoostOn();
				particleHolder.Invoke("SpeedBoostOff", particleHolder.timeout);
			}
			
			if (playerPowerUp.isHealth) {
				particleHolder.HealthUpOn();
				particleHolder.Invoke("HealthUpOff", particleHolder.timeout);
			}
		}
	}

	public void ResetStats() {
		speed = originalSpeed;
		runSpeed = originalRunspeed;
		canDamage = true;
	}

	private void SwitchAnimationBools() {
		if (Animator)
			Animator.SetBool("IsJumping", false);
		if (Animator)
			Animator.SetBool("IsRolling", false);
		if (Animator)
			Animator.SetBool("IsRunning", false);
		if (Animator)
			Animator.SetBool("TakeDamage", true);
	}
}
