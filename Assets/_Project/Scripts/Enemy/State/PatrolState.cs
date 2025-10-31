using UnityEngine;

public class PatrolState : EnemyState
{
    private Vector3 targetPosition;
    private Vector3 lastPlayerPos;

    public PatrolState(EnemyBase enemy) : base(enemy) { }

    private static float speed = EnemyController.patrolSpeed;
    public override void Enter()
    {
        Debug.Log("Patrol enter");
        enemy.animator.SetTrigger("Patrol");
        enemy.GetComponent<EnemyAudioController>()?.SetState("Patrol");
        enemy.agent.speed = speed;
        // Imposta parametro speed per animazioni procedural
        enemy.animator.SetFloat("speed", speed); // da calibrare con procedural
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
            /*waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                PickNewDestination();
                waitCounter = 0;
            }*/
            PickNewDestination();
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
    public override void Exit()
    {
        Debug.Log("Patrol exit");
    }
}
