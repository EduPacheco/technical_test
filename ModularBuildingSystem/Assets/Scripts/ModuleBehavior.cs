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

    [SerializeField] private bool canBeEdited;
    public bool CanBeEdited { get => canBeEdited; }
    [SerializeField] private GameObject overlapError;

    private bool isBuilt = false;

    #endregion

    private void Start()
    {
        if(canBeEdited)
            if (overlapError != null && overlapError.activeSelf)
                overlapError.SetActive(false);
    }

    public void Build()
    {
        if (buildingOverlap)
            Destroy(gameObject);

        isBuilt = true;
    }

    public void Editing(bool e)
    {
        isBuilt = !e;
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
        if (CanOverlap(other))
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
        if (CanOverlap(other))
        {
            if(!buildingOverlap)
                buildingOverlap = true;
            overlapError.SetActive(buildingOverlap);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (CanOverlap(other))
        {
            buildingOverlap = false;
            overlapError.SetActive(buildingOverlap);
        }
    }

    private bool CanOverlap(Collision other)
    {
        return (this.gameObject.CompareTag("Door") || this.gameObject.CompareTag("Window"))
            && (other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("Window"))
            && !isBuilt;
    }
}
