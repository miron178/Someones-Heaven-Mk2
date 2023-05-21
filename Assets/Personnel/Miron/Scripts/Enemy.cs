using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour, IPushable, IDamageable
{
    [SerializeField]
    private float chaseRange = 15;

    [SerializeField]
    private float wanderRadius = 10;
    [SerializeField]
    private float wanderTime = 10;
    [SerializeField]
    private float idleTime = 10;
    [SerializeField]
    private float surprisedTime = 1;
	

	[SerializeField]
    private bool DEBUG_BOOL;
    Vector3 nextPosition;

    public NavMeshAgent agent;

    [SerializeField]
	private bool scaredOfFire = false;

	[SerializeField]
	private Animator enemyAnimator;

    [SerializeField]
    private static string SelectedTag = "Player";

    [SerializeField]
    private float endPushSpeed = 0.1f;

    [SerializeField]
    float radiusToTarget = 1.25f;

    [SerializeField]
    private int attackDamage = 1;

    [SerializeField] [Range(0,1)]
    private float hitChance = 0.5f;

    [SerializeField]
    private float attackRange = 2;

    [SerializeField]
    private float attackCD = 1;
    private float attackTime = 0;

    [SerializeField]
    private float pullRange = 5;
    [SerializeField]
    private float pullDuration = Mathf.Infinity;
    [SerializeField]
    private float pullCD = 3;
    [SerializeField]
    private float pullForce = 1;

    private float pullStart = 0;

	public string nextMethod;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    bool shoot = false;

    [SerializeField]
    float shootMaxAngle = 30;

    [SerializeField]
    Material material;

    [SerializeField]
    Transform gun;

    GameObject[] targets;
    GameObject closest;

    Rigidbody rb;

	private enum State
	{
		IDLE,
		WANDERING,
		CHASING,
		ATTACKING,
		FLEEING,
		SURPRISED,
		PULLING,
		DEAD,
		PUSHED,
    }

    private static string[] stateName =
    {
        "isIdle",
        "isWandering",
        "isChasing",
        "isAttacking",
        "isFleeing",
        "isSurprised",
        "isPulling",
        "isDead",
        "isPushed",
    };

	private State state = State.IDLE;


	//public int speed;

	private void Start()
    {

		nextPosition = transform.position;
		enemyAnimator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //Find the closest target once here
		ClosestTarget();

		switch (state)
        {
            case State.IDLE:
                Idle();
                break;
            case State.WANDERING:
                Wandering();
                break;
            case State.CHASING:
                Chasing();
                break;
            case State.ATTACKING:
                Attacking();
                break;
            case State.FLEEING:
                Fleeing();
                break;
            case State.SURPRISED:
                Surprised();
                break;
            case State.PULLING:
                Pulling();
                break;
            case State.DEAD:
                Dead();
                break;
            case State.PUSHED:
                Pushed();
                break;
            default:
                Debug.LogError("The state is unknown.");
                break;
        }
    }

    private void SetState(State newState)
    {
        //Debug.Log("State: " + state + " --> " + newState);
        state = newState;
        SetAnimationBool(stateName[(int)newState]);

        CancelInvoke(); //TODO: is it a good idea?
    }

    private void StartAgent()
    {
        agent.speed.Equals(3);
        agent.angularSpeed.Equals(120);
        agent.acceleration.Equals(8);
        agent.isStopped = false;
    }

    private void StopAgent()
    {
        agent.speed.Equals(0);
        agent.angularSpeed.Equals(0);
        agent.acceleration.Equals(0);
        agent.isStopped = true;
        agent.SetDestination(transform.position);
    }

    private void SelectNextState()
    {
        if (CanAttack())
        {
            StartAttacking();
        }
        else if (CanPull())
        {
            StartPulling();
        }
        else if (CanChase())
        {
            StartChasing();
        }
        else
        {
            StartIdle();
        }
    }

    private void StartIdle()
    {
        SetState(State.IDLE);
        StopAgent();
        Invoke(nameof(EndIdle), idleTime);
    }

    private void Idle()
    {
        if (closest)
        {
            StartSurprised();
        }
    }

    private void EndIdle()
    {
        StartWandering();
    }

    private void StartWandering()
    {
        SetState(State.WANDERING);
        if (enemyAnimator)
        {
            enemyAnimator.SetBool("isMoving", true);
        }

        // Pick next point that's far enough
        do
        {
            nextPosition = RandomPointGenerator.PointGen(transform.position, wanderRadius);
        } while (Vector3.Distance(nextPosition, transform.position) <= 1.5f);

        StartAgent();
        agent.SetDestination(nextPosition);

        Invoke(nameof(EndWandering), wanderTime);
    }

    private void Wandering()
    {
        if (closest)
        {
            StartSurprised();
        }
    }

    private void EndWandering()
    {
        StartIdle();
    }

    private void StartSurprised()
    {
        SetState(State.SURPRISED);
        StopAgent();
    }

    private void Surprised()
    {
		Invoke(nameof(EndSurprised), surprisedTime);
	}

    private void EndSurprised()
    {
        SelectNextState();
    }

    private void StartChasing()
    {
        SetState(State.CHASING);
        StartAgent();
    }

    private void Chasing()
    {
        if (closest) 
        {
            MoveToClosest();
            if (CanAttack())
            {
                StartAttacking();
            }
            else if (CanPull())
            {
                StartPulling();
            }
        }
        else
        {
            EndChasing();
        }
    }

    private void EndChasing()
    {
        SelectNextState();
    }

    private void StartFleeing()
    {
        SetState(State.FLEEING);
        StartAgent();
    }

	private void Fleeing() {
        if (closest)
        {
            Vector3 away = closest.transform.position - transform.position;
            MoveTo(away);
        }
        else
        {
            EndFleeing();
        }
	}

    private void EndFleeing()
    {
        SelectNextState();
    }

    //private void StartFall() {
    //	state = State.FALLING;
    //}

    GameObject ClosestTarget()
    {
        closest = null;

        float range = Mathf.Max(chaseRange, pullRange, attackRange);

        targets = GameObject.FindGameObjectsWithTag(SelectedTag);

        float distance = Mathf.Infinity;
        Vector3 pos = transform.position;

        foreach (GameObject target in targets)
        {
            Player player = target.GetComponent<Player>();
            if (!player)
                continue;

            Vector3 difference = target.transform.position - pos;
            float currentDistance = difference.sqrMagnitude;

            if (currentDistance > range * range)
            {
                continue; //too far
            }

            if (currentDistance < distance)
            {
                closest = target;
                distance = currentDistance;
            }
        }
        return closest;
	}

	public bool IsPushActive()
    {
        return !agent.enabled;
    }

    public void Push(Vector3 force)
    {
        if (!IsPushActive())
        {
            StartPushed();
        }
        rb.AddForce(force);
    }

    void StartPushed()
    {
        StopAgent();
        agent.enabled = false;
        rb.isKinematic = false;

        //interrupt pull, if active
        //if (state == State.PULLING)
        //{
        //    StopPulling();
        //}

        //stop all pending invokes
        CancelInvoke();

        SetState(State.PUSHED);
    }

    private void Pushed()
    {
        Vector3 horiz = rb.velocity;
        horiz.y = 0;

        //This function can be called before rb.AddForce() actually changes the velocity
        //hence the > 0 check
        bool slowedDown = horiz.sqrMagnitude > 0 && horiz.sqrMagnitude < endPushSpeed * endPushSpeed;
        if (slowedDown)
        {
            EndPushed();
        }
    }

    void EndPushed()
    {
        agent.enabled = true;
        rb.isKinematic = true;

        StartIdle();
	}

    private void LookAt(Vector3 target)
    {
        Vector3 forward = closest.transform.position - transform.position;
        float speed = agent.angularSpeed / 180f * Time.fixedDeltaTime;
        Quaternion look = Quaternion.LookRotation(forward);
        agent.transform.rotation = Quaternion.Slerp(transform.rotation, look, speed);
    }

    private void MoveTo(Vector3 target)
    {
        if (enemyAnimator)
        {
            enemyAnimator.SetBool("isMoving", true);
        }

        agent.SetDestination(target);
        Debug.DrawLine(transform.position, target);

        //rotate towards target
        LookAt(target);
    }

    private void MoveToClosest()
    {
		//radius round target
		Vector3 target = transform.position - closest.transform.position;
        target = target.normalized * radiusToTarget + closest.transform.position;

        //move to target radius
        MoveTo(target);
    }

    private bool TargetInRange()
    {
        if (!closest)
        {
            return false;
        }

        Vector3 distance = transform.position - closest.transform.position;
        return distance.magnitude <= attackRange;
    }

    private bool ShootAngleInRange()
    {
        if (!TargetInRange())
        {
            return false;
        }

        Vector3 direction = closest.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);
        return angle < shootMaxAngle;
    }

    private bool CanAttack()
    {
        return Time.time >= attackTime && TargetInRange() && (!shoot || ShootAngleInRange());
    }

    private void StartAttacking()
    {
        SetState(State.ATTACKING);

        bool hit = Random.value <= hitChance;
        if (hit)
        {
            if (shoot)
            {
                // Fire a bullet towards the player
                Vector3 direction = closest.transform.position - transform.position;
                GameObject bullet = GameObject.Instantiate(bulletPrefab, gun.position, gun.rotation);
                bullet.GetComponent<Bullet>().Direction = direction;
                bullet.GetComponent<Bullet>().Damage = attackDamage;
            }
            else
            {
                Player player = closest.GetComponent<Player>();
                player.TakeDamage(attackDamage);
            }
        }
        attackTime = Time.time + attackCD;

        Invoke(nameof(EndAttacking), 0.2f);
    }

    private void Attacking()
    {
        if (TargetInRange())
        {
            LookAt(closest.transform.position);
        }
        else
        {
            EndAttacking();
        }
    }

    private void EndAttacking()
    {
        SelectNextState();
    }

    private void StartPulling()
    {
        SetState(State.PULLING);
        pullStart = Time.time;
    }

    private void Pulling()
    {
        if (CanAttack())
        {
            StartAttacking();
        }
        else if (CanPull())
        {
            LookAt(closest.transform.position);
            Player player = closest.GetComponent<Player>();
            Vector3 direction = transform.position - closest.transform.position;
            player.Push(direction.normalized * pullForce);
        }
        else
        {
            StopPulling();
        }
    }

    private void StopPulling()
    {
        pullStart = Time.time + pullCD;
        SelectNextState();
    }

    private bool CanChase()
    {
        bool can = false;
        if (closest)
        {
            Vector3 distance = transform.position - closest.transform.position;
            can = distance.magnitude <= chaseRange;
        }
        return can;
    }
    private bool CanPull()
    {
        bool can = true;
        if (!closest ||
            Time.time < pullStart ||
            (state == State.PULLING && Time.time > (pullStart + pullDuration)))
        {
            can = false;
        }
        else
        {
            Vector3 distance = transform.position - closest.transform.position;
            can = distance.magnitude <= pullRange;
        }
        return can;
    }

    private void OnDrawGizmos()
    {
        Color chase = Color.green;
        Color attack = Color.red;
        Color pull = Color.blue;

        Gizmos.color = CanChase() ? chase : chase * 0.5f;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = CanAttack() ? attack : TargetInRange() ? attack * 0.75f : attack * 0.5f;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = CanPull() ? pull : pull * 0.5f;
        Gizmos.DrawWireSphere(transform.position, pullRange);

        if (DEBUG_BOOL == true)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nextPosition);
        }

        if (shoot && TargetInRange())
        {
            Gizmos.color = ShootAngleInRange() ? Color.green : Color.red;

            Vector3 direction = closest.transform.position - transform.position;
            Gizmos.DrawRay(transform.position, direction);
        }

    }

    //TODO: Call this from somewhere!
    private void StartDead()
    {
        SetState(State.DEAD);
        StopAgent();
    }

	private void Dead() 
    {
        //Enjoy!
	}

    private void EndDead()
    {
        //No ressurections
    }

    private void SetAnimationBool(string name, bool value = true)
    {
        if (enemyAnimator)
        {
            foreach (string state in stateName)
            {
                enemyAnimator.SetBool(state, false);
            }
            enemyAnimator.SetBool(name, value);
        }
    }

    void OnDestroy()
    {
        RoomGenerator.Instance.RemoveEnemy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        GameObject.Destroy(this.gameObject);
    }
}
