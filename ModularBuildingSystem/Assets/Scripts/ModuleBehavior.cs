using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleBehavior : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] GameObject defaultModule;
    public GameObject DefaultModule { get => defaultModule; }
    [SerializeField] GameObject[] visuals;
    private int currentVisual = 0;
    public int CurrentVisual { get => currentVisual; }

    private bool buildingOverlap = false;
    public bool BuildingOverlap { get => buildingOverlap; }

    private bool isBuilt = false;

    #endregion

    public void Build()
    {
        isBuilt = true;
    }

    public void ChangeVisuals(ModuleBehavior intakeToMimic)
    {
        if (intakeToMimic.DefaultModule.activeSelf)
            return;
        else
        {
            if (defaultModule.activeSelf)
                defaultModule.SetActive(false);
            else
                visuals[currentVisual].SetActive(false);

            currentVisual = intakeToMimic.CurrentVisual;
            visuals[currentVisual].SetActive(true);
        }
    }

    public void ChangeVisuals(bool intake)
    {
        if (defaultModule.activeSelf)
            defaultModule.SetActive(false);
        else
            visuals[currentVisual].SetActive(false);

        if(intake)
            currentVisual++;
        else
            currentVisual--;

        if (currentVisual >= visuals.Length)
            currentVisual = 0;
        else if(currentVisual < 0)
            currentVisual = visuals.Length - 1;

        visuals[currentVisual].SetActive(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(gameObject.tag) && !isBuilt)
        {
            Destroy(this.gameObject);
        }
    }
}
