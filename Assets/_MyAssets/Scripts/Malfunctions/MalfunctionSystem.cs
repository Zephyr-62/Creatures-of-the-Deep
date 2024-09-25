using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalfunctionSystem : MonoBehaviour
{
    [SerializeField] private SubmarineControlSwitchboard submarineControlSwitchboard;
    [SerializeField] private LightBoard lightBoard;

    [SerializeField] private List<Malfunction> malfunctions;
    
    private List<MalfunctionSymptom> symptoms = new List<MalfunctionSymptom>();
    [SerializeField] private List<Malfunction> currentMalfunctions = new List<Malfunction>();

    public void Collision(Collision collision)
    {

    }

    private void Start()
    {
        foreach (var item in malfunctions)
        {
            Failure(item);
        }
        StartCoroutine(ErrorCodeLoop());
    }

    private void Failure(Malfunction malfunction)
    {
        if (!malfunction) return;

        currentMalfunctions.Add(malfunction);

        ApplySymptoms(malfunction);
    }

    private void ApplySymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if((malfunction.Symptoms & symptom.Id) == symptom.Id)
            {
                symptom.Apply();
            }
        }
    }

    private void RemoveSymptoms(Malfunction malfunction)
    {
        foreach (var symptom in symptoms)
        {
            if ((malfunction.Symptoms & symptom.Id) == symptom.Id)
            {
                symptom.Remove();
            }
        }
    }

    IEnumerator ErrorCodeLoop()
    {
        while (true)
        {
            lightBoard.SetLights(currentMalfunctions[0].ErrorCode);
            currentMalfunctions.Add(currentMalfunctions[0]);
            currentMalfunctions.RemoveAt(0);

            yield return new WaitForSeconds(2f);
            
            lightBoard.SetLights(Malfunction.ErrorMask.None);
            
            yield return new WaitForSeconds(1f);
        }
    }
}
