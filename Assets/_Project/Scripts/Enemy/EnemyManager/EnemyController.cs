using UnityEngine;

[RequireComponent(typeof(EnemyPerception), typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyController : EnemyBase
{
    public static float patrolSpeed = 1.5f;
    public static float chaseSpeed = 5f;
    public static float searchTime = 10f;
    public static float timer = 0f;
    protected override void Start()
    {
        base.Start();
        SwitchState(new PatrolState(this));
    }
}
