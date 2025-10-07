using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("isChasing", true);
        // enemy.animator.SetFloat("speed", 1f); // per procedural animation
    }

    public override void Tick()
    {
        if (enemy.player == null) return;

        enemy.agent.SetDestination(enemy.player.position);

        // Aggiorna speed per procedural animation
        // float currentSpeed = enemy.agent.velocity.magnitude;
        // enemy.animator.SetFloat("speed", currentSpeed);

        if (!enemy.perception.CanSeePlayer())
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
        enemy.animator.SetBool("isChasing", false);
        // enemy.animator.SetFloat("speed", 0f);
    }
}
