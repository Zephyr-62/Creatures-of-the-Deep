using AdvancedEditorTools.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookholderConstraints : MonoBehaviour
{
    [Header("Inputs")]
    [Range(45, 135)]
    public float BaseRotation = 135;
    [Range(0, 0.44f)]
    public float TargetDepth = 0.2f;

    [Header("Configs")]
    [SerializeField] private float TargetHeight = 0.35f;
    [SerializeField] private float IK_Hint_depth = 0.5f;

    [Header("Transforms")]
    [SerializeField] private Transform IK_hint;
    [SerializeField] private Transform IK_target;
    [SerializeField] private Transform PlayerPOV;
    [LineSeparator]
    [SerializeField] private Transform PivotBase;
    [SerializeField] private Transform PivotArm1;
    [SerializeField] private Transform PivotArm2;
    [SerializeField] private Transform PivotBook;

    private Vector3 ArmDir = Vector3.forward;

    // Update is called once per frame
    void Update()
    {
        ArmDir = PivotBook.position - PlayerPOV.position;
        ArmDir = Vector3.Scale(ArmDir, new Vector3(1, 0, 1)).normalized;

        PivotBase.rotation = Quaternion.Euler(0, BaseRotation, 0);
        CalculateTarget();
        CalculateHintPos();
    }

    private void CalculateTarget()
    {
        IK_target.LookAt(PlayerPOV);
        IK_target.position = PivotArm1.position + PivotBase.up * TargetHeight + ArmDir * TargetDepth;
    }

    private void CalculateHintPos()
    {
        IK_hint.position = PivotArm1.position + ArmDir * IK_Hint_depth;
    }
}
