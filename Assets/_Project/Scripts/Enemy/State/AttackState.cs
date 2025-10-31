using UnityEngine;

public class AttackState : EnemyState
{
    private bool isAttacking = false;

    public AttackState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("Attack enter");
        enemy.agent.isStopped = true;

        // Esegue trigger di attacco
        enemy.animator.SetTrigger("Attack");
        isAttacking = true;

        // Blocca movimento player
        PlayerController playerController = enemy.player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        // Se ci fossero animazioni procedural di braccia/gambe, si fermerebbero qui
        // enemy.animator.SetFloat("speed", 0f);
    }

    public override void Tick()
    {
        if (isAttacking && AnimationFinished("Attack"))
        {
            Debug.Log("Player Morto");
            GameManager.Instance.OnPlayerDeath(); // chiama GameManager per la morte del player
            isAttacking = false;
        }
    }

    private bool AnimationFinished(string animName)
    {
        return enemy.animator.GetCurrentAnimatorStateInfo(0).IsName(animName)
            && enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
        // Riattiva eventuali parametri procedural
        // enemy.animator.SetFloat("speed", 0f);
    }
}
