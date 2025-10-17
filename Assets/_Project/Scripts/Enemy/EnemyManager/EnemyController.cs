using UnityEngine;

[RequireComponent(typeof(EnemyPerception), typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyController : EnemyBase
{
    public static float stationaryTimer = 5f;
    protected override void Start()
    {
        base.Start();
        SwitchState(new PatrolState(this));
    }
}
