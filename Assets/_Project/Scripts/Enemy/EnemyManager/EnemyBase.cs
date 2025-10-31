using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public EnemyPerception perception;
    [HideInInspector] public Transform player;
    [HideInInspector] public Animator animator;

    protected EnemyState currentState;

    // Parametri procedural
    [Header("Procedural Animation Settings")]
    public bool useProceduralAnimation = false;  // toggle generale
    // public ProceduralAnimation procedural; 

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        perception = GetComponent<EnemyPerception>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //stato iniziale 
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

        // Aggiorna animazioni procedural se attivo
        if (useProceduralAnimation)
        {
            // procedural.UpdateProcedural(agent.velocity.magnitude);
        }

        // Aggiorna parametro speed Animator per fallback
        /*if (animator != null && agent != null)
        {
            animator.SetFloat("speed", agent.velocity.magnitude);
        }*/
    }
}
