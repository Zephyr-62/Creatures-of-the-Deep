using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookholderConstraints : MonoBehaviour
{
    public float IK_Hint_depth = 0.5f;

    [SerializeField] private Transform IK_hint;
    [SerializeField] private Transform IK_target;
    [SerializeField] private Transform PivotBase;
    [SerializeField] private Transform PivotArm1;



    // Update is called once per frame
    void Update()
    {
        CalculateHintPos();
    }

    private void CalculateHintPos()
    {
        var pivotBaseAngle = PivotBase.localRotation.eulerAngles.y*Mathf.PI/180.0f;
        IK_hint.position = PivotArm1.position;
        IK_hint.localPosition += new Vector3(Mathf.Sin(pivotBaseAngle)*IK_Hint_depth, 0, -Mathf.Cos(Mathf.PI - pivotBaseAngle)*IK_Hint_depth);
    }
}
