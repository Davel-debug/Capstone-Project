using UnityEngine;

public class SearchState : EnemyState
{
    private Vector3 searchCenter;
    private float searchTime = 10f;
    private float timer;
    private float stationaryTimer;
    private Vector3 lastPlayerPos;

    public SearchState(EnemyBase enemy) : base(enemy) { }

    public override void Enter()
    {
        searchCenter = AIManager.Instance.GetApproximatePlayerPosition();
        enemy.agent.SetDestination(searchCenter);
        timer = 0f;
        // enemy.animator.SetFloat("speed", 0.5f); // per procedural animation
    }

    public override void Tick()
    {
        timer += Time.deltaTime;

        if (enemy.perception.CanSeePlayer())
        {
            enemy.SwitchState(new ChaseState(enemy));
            return;
        }

        if (timer > searchTime)
        {
            enemy.SwitchState(new PatrolState(enemy));
            return;
        }

        // Calcola quanto il player rimane fermo
        if (Vector3.Distance(enemy.player.position, lastPlayerPos) < 0.5f)
        {
            stationaryTimer += Time.deltaTime;
            if (stationaryTimer > 5f) // tempo configurabile
            {
                // Riprende posizione esatta del player e ride (procedural non ancora implementata)
                // enemy.animator.SetTrigger("Laugh");
                enemy.SwitchState(new ChaseState(enemy));
            }
        }
        else
        {
            stationaryTimer = 0f;
        }

        lastPlayerPos = enemy.player.position;

        // Aggiorna speed per procedural animation
        // float currentSpeed = enemy.agent.velocity.magnitude;
        // enemy.animator.SetFloat("speed", currentSpeed);
    }
}
