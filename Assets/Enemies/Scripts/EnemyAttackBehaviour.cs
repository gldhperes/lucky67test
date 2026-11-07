
using UnityEngine;

public abstract class EnemyAttackBehaviour : MonoBehaviour
{
    protected EnemyBehaviour enemy;
    protected Transform target;

    public virtual void Initialize(
        EnemyBehaviour enemy,
        Transform target)
    {
        this.enemy = enemy;
        this.target = target;
    }

    public abstract void PerformAttack();
}
