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
    [Header("Appearance")]
    [SerializeField] GameObject defaultModule;
    public GameObject DefaultModule { get => defaultModule; }

    [SerializeField] GameObject[] visuals;

    private int currentVisual = 0;
    public int CurrentVisual { get => currentVisual; }

    [Header("Editing and building")]
    [SerializeField] ModuleType myType;
    public ModuleType MyType { get => myType; }

    private bool buildingOverlap = false;
    public bool BuildingOverlap { get => buildingOverlap; }

    [SerializeField] private GameObject overlapError;
    [SerializeField] private bool canBeEdited;
    public bool CanBeEdited { get => canBeEdited; }

    private bool isBuilt = false;

    #endregion

    /// <summary>
    /// If can be edited, hide overlap feedback gameobject
    /// </summary>
    private void Start()
    {
        //Disables any active module skins
        foreach (GameObject v in visuals)
        {
            if (v.activeSelf)
            {
                v.SetActive(false);
            }
        }

        defaultModule.SetActive(true);

        if (canBeEdited)
            if (overlapError != null && overlapError.activeSelf)
                overlapError.SetActive(false);
    }

    /// <summary>
    /// Change Module from temporary to permanent prop,
    /// if overlapping with another prop, the new prop is destroyed
    /// </summary>
    public void Build()
    {
        if (buildingOverlap)
            Destroy(gameObject);

        isBuilt = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="intakeToMimic"></param>
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

    /// <summary>
    /// Changes the appearance of the module for another in the array
    /// </summary>
    /// <param name="intake">True goes to the next one in the array, false goes to the previous one</param>
    public void ChangeVisuals(bool intake)
    {
        if (defaultModule.activeSelf)//If its the first change, disable default skin
            defaultModule.SetActive(false);
        else //Disables current one
            visuals[currentVisual].SetActive(false);

        //Checks which one the user wants next
        if(intake)
            currentVisual++;
        else
            currentVisual--;

        //Makes sure the index stays in bounds of the array
        if (currentVisual >= visuals.Length)
            currentVisual = 0;
        else if(currentVisual < 0)
            currentVisual = visuals.Length - 1;

        //Enables it
        visuals[currentVisual].SetActive(true);
    }

    /// <summary>
    /// Swap Between edit and built
    /// </summary>
    /// <param name="e">True if is being edited</param>
    public void EditMode(bool e)
    {
        isBuilt = !e;

        if(isBuilt)
        {
            buildingOverlap = false;
            overlapError.SetActive(buildingOverlap);
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        // Check if overlapping

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
        // Check if stopped overlapping

        if (CanOverlap(other)) 
        {
            buildingOverlap = false;
            overlapError.SetActive(buildingOverlap);
        }
    }

    /// <summary>
    /// Check if there is a collision with another prop that can be conflicting in current position
    /// </summary>
    /// <param name="other">The other prop to compare to</param>
    /// <returns>True if colliding and have these tags, false if not</returns>
    private bool CanOverlap(Collision other)
    {
        return (this.gameObject.CompareTag("Door") || this.gameObject.CompareTag("Window"))
            && (other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("Window"))
            && !isBuilt;
    }
}
