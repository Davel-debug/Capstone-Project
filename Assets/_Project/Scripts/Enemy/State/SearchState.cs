using UnityEngine;

public class SearchState : EnemyState
{
    private Vector3 searchCenter;
    private float searchTime = EnemyController.searchTime;
    private float timer = EnemyController.timer;
    private Vector3 lastPlayerPos;


    public SearchState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("Search enter");
        enemy.animator.SetTrigger("Search");
        enemy.GetComponent<EnemyAudioController>()?.SetState("Search");
        searchCenter = AIManager.Instance.GetApproximateSearchPlayerPosition();
        enemy.agent.SetDestination(searchCenter);
        timer = 0f;
        enemy.animator.SetFloat("speed", 0.5f); // per procedural animation
    }

    public override void Tick()
    {
        timer += Time.deltaTime;

        if (!AIManager.Instance.PlayerMoved() || enemy.perception.CanSeePlayer())
        {
            enemy.SwitchState(new ChaseState(enemy));
            return;
        }

        if (timer > searchTime)
        {
            enemy.SwitchState(new PatrolState(enemy));
            return;
        }

        lastPlayerPos = enemy.player.position;

        // Aggiorna speed per procedural animation
        // float currentSpeed = enemy.agent.velocity.magnitude;
        // enemy.animator.SetFloat("speed", currentSpeed);
    }
    public override void Exit()
    {
        Debug.Log("Search exit");
    }
    
}
