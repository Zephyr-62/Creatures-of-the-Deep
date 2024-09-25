using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Malfunction;

public class MalfunctionSymptom : MonoBehaviour
{
    [SerializeField] private SymptomMask id;
    private int count;

    public SymptomMask Id => id;

    public void Apply()
    {
        count++;
        if(count == 1)
        {
            SendMessage("OnStopFunctioning");
        }
    }

    public void Remove()
    {
        count--;
        count = Mathf.Max(count, 0);
        if(count == 0)
        {
            SendMessage("OnResumeFunctioning");
        }
    }
}
