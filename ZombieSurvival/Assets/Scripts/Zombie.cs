using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }

    public Transform target;
    public LayerMask targetLayer;

    public float traceDistance = 10f;
    public float attackDistance = 1.2f;

    public ParticleSystem bloodEffect;
    public Collider zombieCollider;
    public Renderer zombieRenderer;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private NavMeshAgent agent;
    private Animator zombieAnimator;
    private AudioSource zombieAudioSource;

    public float damage = 10f;
    private float lastAttackTime;
    public float attackInterval = 1f;
    private Status currentStatus;

    public static readonly int IdHasTarget = Animator.StringToHash("HasTarget");
    public static readonly int IdDie = Animator.StringToHash("Die");
    public Status CurrentStatus
    {
        get => currentStatus;
        set
        {
            currentStatus = value;
            switch (currentStatus)
            {
                case Status.Idle:
                    zombieAnimator.SetBool(IdHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    zombieAnimator.SetBool(IdHasTarget, true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    zombieAnimator.SetBool(IdHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Die:
                    zombieAnimator.SetTrigger(IdDie);
                    agent.isStopped = true;
                    zombieCollider.enabled = false;
                    zombieAudioSource.PlayOneShot(deathClip);
                    break;
            }
        }
    }

    public void Setup(ZombieData data)
    {
        startingHealth = data.maxHp;
        damage = data.damage;
        agent.speed = data.speed;
        zombieRenderer.material.color = data.skinColor;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (CurrentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;
            case Status.Trace:
                UpdateTrace();
                break;
            case Status.Attack:
                UpdateAttack();
                break;
            case Status.Die:
                UpdateDie();
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        agent.enabled = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        zombieCollider.enabled = true;

        CurrentStatus = Status.Idle;
    }

    private void OnDisable()
    {
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }
    private void UpdateDie()
    {

    }

    private void UpdateAttack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > attackDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        if (Time.time >= lastAttackTime + attackInterval)
        {
            lastAttackTime = Time.time;
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                var hitNormal = (transform.position - target.position).normalized;
                damageable.OnDamage(damage, transform.position, hitNormal);
            }
        }
    }

    private void UpdateTrace()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            target = null;
            CurrentStatus = Status.Idle;
            return;
        }
        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            CurrentStatus = Status.Attack;
            return;
        }
        agent.SetDestination(target.position);
    }

    private void UpdateIdle()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        target = FindTarget(traceDistance);
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        Transform closet = null;
        float closetDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < closetDistance)
            {
                closetDistance = distance;
                closet = colliders[i].transform;
            }
        }
        return closet;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDeath)
        {
            return;
        }
        base.OnDamage(damage, hitPoint, hitNormal);

        zombieAudioSource.PlayOneShot(hitClip);
        bloodEffect.transform.position = hitPoint;
        bloodEffect.transform.forward = hitNormal;
        bloodEffect.Play();
    }

    public override void Die()
    {
        base.Die();

        CurrentStatus = Status.Die;
    }

}
