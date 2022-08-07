using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleType
{
    WALL,
    FLOOR,
    DOOR,
    WINDOW
}

public class ModuleBehavior : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] ModuleType myType;
    public ModuleType MyType { get => myType; }

    [SerializeField] GameObject defaultModule;
    public GameObject DefaultModule { get => defaultModule; }

    [SerializeField] GameObject[] visuals;

    private int currentVisual = 0;
    public int CurrentVisual { get => currentVisual; }

    private bool buildingOverlap = false;
    public bool BuildingOverlap { get => buildingOverlap; }

    [SerializeField] private GameObject overlapError;

    private bool isBuilt = false;

    #endregion

    private void Start()
    {
        if (overlapError.activeSelf)
            overlapError.SetActive(false);
    }

    public void Build()
    {
        if (buildingOverlap)
            Destroy(gameObject);

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
        if ((this.gameObject.CompareTag("Door") || this.gameObject.CompareTag("Window"))
            && other.gameObject.CompareTag(gameObject.tag)
            && !isBuilt)
        {
            buildingOverlap = true;
            overlapError.SetActive(buildingOverlap);
            return;
        }

        if (other.gameObject.CompareTag(gameObject.tag) && !isBuilt)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if ((this.gameObject.CompareTag("Door") || this.gameObject.CompareTag("Window")) 
            && other.gameObject.CompareTag(gameObject.tag)
            && !isBuilt)
        {
            if(!buildingOverlap)
                buildingOverlap = true;
            overlapError.SetActive(buildingOverlap);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag(gameObject.tag) 
            && (this.gameObject.CompareTag("Door") || this.gameObject.CompareTag("Window"))
            && !isBuilt)
        {
            buildingOverlap = false;
            overlapError.SetActive(buildingOverlap);
        }
    }
}
