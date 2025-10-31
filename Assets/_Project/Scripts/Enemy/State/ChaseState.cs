using UnityEditor;
using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyBase enemy) : base(enemy) { }

    private static float speed = EnemyController.chaseSpeed;
    public override void Enter()
    {
        Debug.Log("Chase enter");
        enemy.animator.SetTrigger("Chase");
        
        enemy.agent.speed = speed ;
        
        enemy.animator.SetFloat("speed", speed); // per procedural animation
    }

    public override void Tick()
    {
        if (enemy.player == null) return;

        enemy.agent.SetDestination(enemy.player.position);

        // Aggiorna speed per procedural animation
        // float currentSpeed = enemy.agent.velocity.magnitude;
        // enemy.animator.SetFloat("speed", currentSpeed);

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 2f)
        {
            enemy.SwitchState(new AttackState(enemy));
        }
        if (!enemy.perception.CanSeePlayer())
        {
            enemy.SwitchState(new SearchState(enemy));
            return;
        }

    }

    public override void Exit()
    {
        Debug.Log("Chase exit"); 
    }
}
