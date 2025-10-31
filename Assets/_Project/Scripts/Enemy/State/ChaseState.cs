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
        enemy.GetComponent<EnemyAudioController>()?.SetState("Chase");

        enemy.agent.speed = speed ;
        
        enemy.animator.SetFloat("speed", speed); // per procedural animation
    }

    public override void Tick()
    {
        if (enemy.player == null) return;

        if (enemy.perception.CanSeePlayer())
        {
            enemy.agent.SetDestination(enemy.player.position);
        }
        else if (enemy.perception.HasRecentSight())
        {
            enemy.agent.SetDestination(enemy.perception.GetLastKnownPosition());
        }
        else
        {
            enemy.SwitchState(new SearchState(enemy));
            return;
        }

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 2f)
        {
            enemy.SwitchState(new AttackState(enemy));
        }
    }


    public override void Exit()
    {
        Debug.Log("Chase exit"); 
    }
}
