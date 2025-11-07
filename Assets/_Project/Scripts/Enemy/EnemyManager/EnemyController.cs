using UnityEngine;

[RequireComponent(typeof(EnemyPerception), typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyController : EnemyBase
{
    public static float patrolSpeed = 1.5f;
    public static float chaseSpeed = 5f;
    public static float searchTime = 10f;
    public static float timer = 0f;

    public GameObject canvas;

    protected override void Start()
    {
        base.Start();

        this.canvas = canvas;

        if (canvas != null)
            canvas.SetActive(true);

        SwitchState(new PatrolState(this));
    }
}
