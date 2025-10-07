using UnityEngine;

public class PatrolState : EnemyState
{
    private Vector3 targetPosition;
    private float waitTime = 2f;
    private float waitCounter;

    public PatrolState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        // Imposta parametro speed per animazioni procedural
        // enemy.animator.SetFloat("speed", 0.5f); // da calibrare con procedural
        PickNewDestination();
    }

    public override void Tick()
    {
        // Controllo percezione player
        if (enemy.perception.CanSeePlayer())
        {
            enemy.SwitchState(new ChaseState(enemy));
            return;
        }

        // Movimento verso destinazione
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                PickNewDestination();
                waitCounter = 0;
            }
        }

        // Aggiorna parametro speed per procedural animation
        // float currentSpeed = enemy.agent.velocity.magnitude;
        // enemy.animator.SetFloat("speed", currentSpeed);
    }

    private void PickNewDestination()
    {
        targetPosition = AIManager.Instance.GetApproximatePlayerPosition(); // da AIManager
        enemy.agent.SetDestination(targetPosition);
    }
}
