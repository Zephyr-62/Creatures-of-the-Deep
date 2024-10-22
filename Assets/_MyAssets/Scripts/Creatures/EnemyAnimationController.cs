using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAIController))]
public abstract class EnemyAnimationController<T> : MonoBehaviour where T : EnemyAIController
{
    [HideInInspector]
    public T AIController;

    virtual protected void Start()
    {
        AIController = GetComponent<T>();
        AIController.OnStateChanged += OnAIStateChanged;
        Init();

    }

    protected virtual void Init() { }
    protected virtual void OnAIStateChanged(EnemyAIController.State newState) { }

}
