using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAnimationController : MonoBehaviour
{
    [HideInInspector]
    public EnemyAIController AIController;

    public virtual void Idle() { }
    public virtual void Patrol() { }
    public virtual void Chase() { }
    public virtual void Attack() { }
    public virtual void Search() { }

}
