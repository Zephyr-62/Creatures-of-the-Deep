using AdvancedEditorTools.Attributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAIController : EnemyAIController
{

    public Vector3 GetMovingDir() => MovingDirection;

    public Transform GetChaseTarget() => ChaseTarget;

    public override void HitChaseTarget()
    {
        base.HitChaseTarget();
        // ChaseTarget.gameObject.SetActive(false);
    }

}
