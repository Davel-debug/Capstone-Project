using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public EnemyPerception perception;
    [HideInInspector] public Transform player;
    [HideInInspector] public Animator animator;

    protected EnemyState currentState;

    [Header("Procedural Animation Settings")]
    public bool useProceduralAnimation = false;  // toggle generale
    // public ProceduralAnimation procedural; 


    //Anti-Stuck Settings

    [Header("Anti-Stuck Settings")]
    [Tooltip("Ogni quanto controllare se il nemico è bloccato (in secondi).")]
    public float stuckCheckInterval = 2f;

    [Tooltip("Distanza minima che deve percorrere per non essere considerato bloccato.")]
    public float stuckDistanceThreshold = 0.3f;

    [Tooltip("Tempo massimo (in secondi) in cui può restare fermo.")]
    public float stuckTimeLimit = 6f;

    [Tooltip("Distanza minima dal player per il teletrasporto.")]
    public float minTeleportDistance = 10f;

    [Tooltip("Distanza massima dal player per il teletrasporto.")]
    public float maxTeleportDistance = 25f;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float checkTimer = 0f;


    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        perception = GetComponent<EnemyPerception>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        lastPosition = transform.position;

        // Stato iniziale 
        SwitchState(new PatrolState(this));
    }

    public void SwitchState(EnemyState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    protected virtual void Update()
    {
        if (currentState != null)
            currentState.Tick();

        if (useProceduralAnimation)
        {
            // procedural.UpdateProcedural(agent.velocity.magnitude);
        }

        HandleAntiStuck();
    }


    // Anti-Stuck Logic
    private void HandleAntiStuck()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer < stuckCheckInterval) return;
        checkTimer = 0f;

        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Se non si è mosso abbastanza
        if (movedDistance < stuckDistanceThreshold)
        {
            stuckTimer += stuckCheckInterval;

            if (stuckTimer >= stuckTimeLimit)
            {
                TryTeleportAwayFromPlayer();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    private void TryTeleportAwayFromPlayer()
    {
        if (player == null) return;

        for (int i = 0; i < 20; i++)
        {
            // Posizione casuale sul NavMesh
            Vector3 randomDirection = Random.insideUnitSphere * maxTeleportDistance;
            randomDirection.y = 0f;
            Vector3 randomPosition = transform.position + randomDirection;

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                float distanceFromPlayer = Vector3.Distance(hit.position, player.position);
                if (distanceFromPlayer >= minTeleportDistance)
                {
                    agent.Warp(hit.position);
                    Debug.Log($"[EnemyBase] Teletrasportato per anti-stuck -> {hit.position}");
                    return;
                }
            }
        }

        Debug.LogWarning("[EnemyBase] Nessuna posizione valida trovata per il teletrasporto.");
    }
}
