using UnityEngine;

[RequireComponent(typeof(EnemyPerception), typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyController : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        SwitchState(new PatrolState(this));
    }
}
