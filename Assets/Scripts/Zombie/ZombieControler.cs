using Interfaces;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public float UpdateRate = 0.1f;

    [SerializeField]
    private Animator Animator = null;

    NavMeshAgent Agent;

    [SerializeField]
    private SphereCollider[] AttackColider;
    public Vector3 attackradius;
    public int attackDamage = 10;
    public bool ReadyToAttack = true;
    bool attackAnimationStarted = false;
    bool attackAnimationHasStarted = false;

    public EnemyState DefaultState; 
    public EnemyState _state;
    public EnemyState oldState;
    private Coroutine FollowCoroutine;

    public float increaseSpeed = 0f;
    public GameObject medKit;

    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(value);
        }
    }

    public delegate void StateChangeEvent(EnemyState newState);
    public event StateChangeEvent OnStateChange;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;
    float currentSpeed;
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        NavMesh.pathfindingIterationsPerFrame = 600;
        Agent.speed += increaseSpeed;
        currentSpeed = Agent.speed;
        OnStateChange += HandleStateChange;
        OnStateChange?.Invoke(EnemyState.Run);
    }

    void Update()
    {
        if (GameObject.FindWithTag("Player") == null)
        {
            OnStateChange?.Invoke(EnemyState.Idle);
            return;
        }

        if(Agent.enabled)
        {
            if (Vector3.Distance(Agent.transform.position, GameObject.FindWithTag("Player").transform.position) <= 2f)
            {
                OnStateChange?.Invoke(EnemyState.Attacking);
            }
            else
            {
                OnStateChange?.Invoke(EnemyState.Run);
            }
        }
    }

    public void HandleStateChange(EnemyState newState)
    {
        if (oldState != newState)
        {
            
            if (FollowCoroutine != null)
            {
                StopCoroutine(FollowCoroutine);
            }

            if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
            }
            if (oldState == EnemyState.Run)
            {
                Animator.SetBool("run", false);
            }
            if (oldState == EnemyState.Attacking)
            {
                Animator.SetInteger("attack", 0);
                attackAnimationStarted = false;
            }

            oldState = newState;
            switch (newState)
            {
                case EnemyState.Idle:
                    FollowCoroutine = StartCoroutine(IdleMotion());
                    break;
                case EnemyState.Attacking:
                    FollowCoroutine = StartCoroutine(Attack());
                    break;
                case EnemyState.Run:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
                case EnemyState.Dead:
                    Animator.SetInteger("die",Random.Range(1,3));
                    FollowCoroutine = StartCoroutine(Die());
                    break;
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        
        Agent.speed = 0;
        Agent.transform.LookAt(GameObject.FindWithTag("Player").transform.position);
        while (true)
        {
            if (!attackAnimationStarted)
            {
                Animator.SetInteger("attack", Random.Range(1, 4));
                StartCoroutine(timerAnimationHasStarted());
                attackAnimationStarted = true;
            }
            if (attackAnimationHasStarted)
            {
                foreach (SphereCollider col in AttackColider)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(col.transform.position, col.radius);
                    foreach (Collider hit in hitColliders)
                    {
                        if (hit.gameObject.TryGetComponent(out IDamageable damageable) && ReadyToAttack)
                        {
                            damageable.TakeDamage(attackDamage);
                            StartCoroutine(AttackInProgress());
                        }
                    }
                }
            }
            yield return Wait;
        }
    }
    IEnumerator timerAnimationHasStarted()
    {
        attackAnimationHasStarted = false;
        float timer = 0;
        while (timer < 0.5)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        attackAnimationHasStarted = true;
    }
    private IEnumerator AttackInProgress()
    {
        ReadyToAttack = false;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ReadyToAttack = true;
    }
    private IEnumerator IdleMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        Agent.speed *= IdleMovespeedMultiplier;

        while (true)
        {
            Animator.SetBool("idle", transform.position == Agent.destination);
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return Wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;
                
                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return Wait;
        }
    }

    private IEnumerator Die()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
       
        while (true)
        {
            Agent.isStopped = true;
            
            Invoke("destroyZombie", 5f);

            yield return Wait;
        }
    }

    public void destroyZombie()
    {
        DropMedKit();
        Destroy(gameObject);
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        if(Agent.speed != currentSpeed)
        {
            Agent.speed = currentSpeed;
        }
        while (true)
        {
            if (Agent.enabled)
            {
                if (GameObject.FindWithTag("Player") != null)
                {
                    GameObject player = GameObject.FindWithTag("Player");
                    Agent.SetDestination(player.transform.position);
                    Animator.SetBool("run", true);
                }
            }
            yield return Wait;
        }
    }

    private void DropMedKit()
    {
        int random = Random.Range(0, 100);
        if (random < 5)
        {
            Instantiate(medKit, transform.position, Quaternion.identity);
        }
    }
}
