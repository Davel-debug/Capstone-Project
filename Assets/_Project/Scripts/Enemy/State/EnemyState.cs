using UnityEngine;

public abstract class EnemyState
{
    protected EnemyBase enemy;
    protected EnemyState(EnemyBase enemy) { this.enemy = enemy; }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }
}
