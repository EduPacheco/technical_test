using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References: https://youtu.be/0jTPKz3ga4w, https://youtu.be/bawSh3WbxjU
/// </summary>
/// 
public enum GameState
{
    CREATE,
    MOVE,
    CUSTOMIZE,
    DEMOLISH
}

public class MouseTarget : MonoBehaviour
{
    #region SINGLETON
    public static MouseTarget instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region VARIABLES
    [Header("User movement")]
    [SerializeField] private float nodeSize = .5f;

    [SerializeField] private Camera mainCam;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask physicsLayer;
    private LayerMask inUseLayerMask;

    [SerializeField] private GameObject[] pointers;
    private GameObject currentPointer;

    [Header("Create and Edit")]
    private List<GameObject> creationBuffer = new List<GameObject>();

    private GameObject prefabModule;

    private ModuleType currentModuleType;
    private GameState currentBuildState;

    private bool isCreating = false;
    private ModuleBehavior isBeingEdited;

    private Vector3 editReturnPos;
    private Quaternion editReturnRot;

    private Vector3 previousPosition;
    #endregion

    /// <summary>
    /// Reset mouse cursor in game to default
    /// </summary>
    private void Start()
    {
        //Disables any active mouse pointer
        foreach (GameObject p in pointers)
        {
            if (p.activeSelf)
            {
                p.SetActive(false);
            }
        }

        currentPointer = pointers[0];
        currentPointer.SetActive(true);
    }

    private void Update()
    {
        MouseRaycast();
    }

    /// <summary>
    /// Projects raycast that follows mouse movement
    /// </summary>
    private void MouseRaycast()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, inUseLayerMask))
        {
            //Show creation Pointer
            if (currentPointer != null && !currentPointer.activeSelf)
            {
                currentPointer.SetActive(true);
            }

            if (hit.transform.gameObject == null)
                return;

            switch (currentBuildState)
            {
                case GameState.CREATE:
                    CreationMode(hit);
                    break;
                case GameState.MOVE:
                    MoveModulesMode(hit);
                    break;
                case GameState.CUSTOMIZE:
                    CustomizeMode(hit);
                    break;
                case GameState.DEMOLISH:
                    DemolishMode(hit);
                    break;
            }
        }
        else
        {
            //Hide cursor if not on bounds
            if (currentPointer != null && currentPointer.activeSelf)
            {
                currentPointer.SetActive(false);
            }
        }

        //Ends safely any creation operation when releasing the left mouse button
        if (Input.GetMouseButtonUp(0))//Mouse Left Click
        {
            ToggleCreationMode(false);
            isBeingEdited = null;
        }
    }

    /// <summary>
    /// Force clear any creation operation on right mouse button release
    /// </summary>
    private void RightClickForceClear()
    {
        if (Input.GetMouseButtonDown(1))//Mouse Right Click
        {
            switch (currentBuildState)
            {
                case GameState.CREATE:
                    //Clear any module created on previous creation click
                    ClearCreationBuffer();
                    break;
            }
        }
    }

    /// <summary>
    /// Creates current prefab module, depending on the type of module, can create multiple or single gameObjects
    /// </summary>
    /// <param name="target">Target position to create</param>
    private void CreationMode(RaycastHit target)
    {
        //Snapping to grid Position
        Vector3 t = target.point;

        Vector3 snappingGrid = new Vector3(
                Mathf.RoundToInt(t.x / nodeSize) * nodeSize,
                0f,
                Mathf.RoundToInt(t.z / nodeSize) * nodeSize);

        transform.position = snappingGrid;

        switch (currentModuleType)
        {
            //Hold and drag for Group Module Creation
            case ModuleType.WALL:
            case ModuleType.FLOOR:
                if (snappingGrid != previousPosition)
                {
                    if (isCreating)
                        CreateModule(snappingGrid, false);

                    previousPosition = snappingGrid;
                }

                if (Input.GetMouseButtonDown(0))//Mouse Left Click
                {
                    ToggleCreationMode(true);
                }
                break;

            //Hold and position single Module Creation
            case ModuleType.DOOR:
            case ModuleType.WINDOW:
                if (Input.GetMouseButtonDown(0))//Mouse Left Click
                {
                    ToggleCreationMode(true);
                    CreateModule(snappingGrid, true);
                }

                if (Input.GetMouseButton(0))
                {

                    if (creationBuffer.Count == 0)
                        return;

                    creationBuffer[0].transform.position = transform.position; //Recently created follow mouse

                    //Rotate 90º with mouse scrollwheel
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                        creationBuffer[0].transform.Rotate(0f, 90f, 0f);
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                        creationBuffer[0].transform.Rotate(0f, -90f, 0f);
                }

                //Stop creation and finish building
                if (Input.GetMouseButtonUp(0))//Mouse Left Click
                    ToggleCreationMode(false);

                break;
        }

        RightClickForceClear();
    }

    /// <summary>
    /// Clears any modules inside the creation buffer that were created and stops creating
    /// </summary>
    private void ClearCreationBuffer()
    {
        if (isCreating)
        {
            foreach (GameObject g in creationBuffer)
            {
                Destroy(g);
            }

            creationBuffer.Clear();
        }

        isCreating = false;
        return;
    }

    /// <summary>
    /// Start creating or Stop creating and build any modules in the creation Buffer
    /// </summary>
    /// <param name="t">Update if creating or not</param>
    private void ToggleCreationMode(bool t)
    {
        if (isCreating)
        {
            foreach (GameObject g in creationBuffer)
            {
                if (g != null)
                    g.GetComponent<ModuleBehavior>().Build();
            }

            creationBuffer.Clear();
        }
        isCreating = t;
    }

    /// <summary>
    /// Picks up desired gameObject, moves it around and rotates it
    /// </summary>
    /// <param name="target">Target module to edit position and rotation</param>
    private void MoveModulesMode(RaycastHit target)
    {     
        if (isBeingEdited != null)//Snap to grid move if has anything picked up
        {
            Vector3 t = target.point;

            Vector3 snappingGrid = new Vector3(
                    Mathf.RoundToInt(t.x / nodeSize) * nodeSize,
                    0f,
                    Mathf.RoundToInt(t.z / nodeSize) * nodeSize);

            transform.position = snappingGrid;

            if (inUseLayerMask != groundLayer)
                inUseLayerMask = groundLayer;
        }
        else // Free movement
        {
            transform.position = target.point;

            if (inUseLayerMask != physicsLayer)
                inUseLayerMask = physicsLayer;
        }

        ModuleBehavior targetModule = target.transform.gameObject.GetComponent<ModuleBehavior>();

        //Starts editing, saves original gameObject position and rotation
        if (Input.GetMouseButtonDown(0))//Mouse Left Click
        {
            if (!targetModule || !targetModule.CanBeEdited)
                return;

            isBeingEdited = targetModule;
            isBeingEdited.EditMode(true);
            editReturnPos = isBeingEdited.transform.position;
            editReturnRot = isBeingEdited.transform.rotation;
        }

        //Moves and rotates gameObject around
        if (Input.GetMouseButton(0))
        {
            if (isBeingEdited == null)
                return;

            isBeingEdited.transform.position = transform.position;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                isBeingEdited.transform.Rotate(0f, 90f, 0f);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                isBeingEdited.transform.Rotate(0f, -90f, 0f);
        }

        //Resets gameObject position and rotation to its original transform before the editing
        if (Input.GetMouseButtonDown(1))//Mouse Right Click
        {
            isBeingEdited.transform.position = editReturnPos;
            isBeingEdited.transform.rotation = editReturnRot;

            isBeingEdited.EditMode(false);
            isBeingEdited = null;

            return;
        }

        //Finishes editing and builds the gameObject again if not overlaping
        if (Input.GetMouseButtonUp(0))
        {
            if(isBeingEdited != null && isBeingEdited.BuildingOverlap)
            {
                isBeingEdited.transform.position = editReturnPos;
                isBeingEdited.transform.rotation = editReturnRot;
            }

            isBeingEdited.EditMode(false);
            isBeingEdited = null;
        }
    }

    /// <summary>
    /// Customize target module, can edit one or multiple at a time
    /// </summary>
    /// <param name="target">Target module to be customized</param>
    private void CustomizeMode(RaycastHit target)
    {
        transform.position = target.point; //Update mouse cursor position

        ModuleBehavior targetModule = target.transform.gameObject.GetComponent<ModuleBehavior>();

        if (!targetModule) // Check if it's a module that can be customized
            return;

        //Left and Right mouse button cycles the customize array 
        //in diferent directions but same result, the modules changes appearence
        //i.e. a circular colour palette

        if (Input.GetMouseButtonDown(0))//Mouse Left Click
        {
            targetModule.ChangeVisuals(true);
            isBeingEdited = targetModule;
        }

        if (Input.GetMouseButtonDown(1))//Mouse Right Click
        {
            targetModule.ChangeVisuals(false);
            isBeingEdited = targetModule;
        }

        //While holding the mouse button, the user can mass customize, 
        //by draging on top of the same type of module, 
        //i.e. painting a wall and holing, draging on top of other walls will paint them with the same colour
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            //i.e. IF the current wall exists 
            //&& the target wall is diferent previous
            //&& the target and previous wall have the same tag wall
            if (isBeingEdited
                && isBeingEdited != targetModule
                && isBeingEdited.tag == targetModule.transform.tag)
            {
                targetModule.ChangeVisuals(isBeingEdited);
            }

            isBeingEdited = targetModule;

            return;
        }

        //Stop the hold and drag mass customaize
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isBeingEdited = null;
        }
    }

    /// <summary>
    /// Remove / Delete modules Mode
    /// </summary>
    /// <param name="target">Target module to be destroyed</param>
    private void DemolishMode(RaycastHit target)
    {
        transform.position = target.point; // update mouse cursor position

        if (Input.GetMouseButtonDown(0))//Mouse Left Click
        {
            //Check if it's a module that can be destroyed
            ModuleBehavior targetModule = target.transform.gameObject.GetComponent<ModuleBehavior>();
            if (targetModule)
                Destroy(targetModule.gameObject);
        }
    }

    /// <summary>
    /// Create desired gameObject
    /// </summary>
    /// <param name="targetPos">Position to be facing</param>
    /// <param name="follow">Shoud it follow the cursor around or stay in place when created</param>
    private void CreateModule(Vector3 targetPos, bool follow)
    {
        //Calculate direction the cursor is facing
        Vector3 dir = (targetPos - previousPosition).normalized;

        if (!prefabModule) //if there is no prefab select, return
            return;

        //Create GameObject without parent
        GameObject module = Instantiate(prefabModule, previousPosition, Quaternion.identity, null);

        if (!follow)//If it shouldn't follow the cursor around apply previously calculated direction
            module.transform.forward = dir;

        //Add to the creation array
        creationBuffer.Add(module);
    }

    /// <summary>
    /// Update which state the game is currently in, and update cursor to display it
    /// </summary>
    /// <param name="newState"></param>
    public void UpdateMouseTargetFunctionality(GameState newState)
    {
        currentBuildState = newState;

        //Change user mouse pointer
        if (currentPointer != null)
            currentPointer.SetActive(false);
        currentPointer = pointers[(int)currentBuildState];
        currentPointer.SetActive(true);

        //Change Layer Mask Raycast Target
        switch (currentBuildState)
        {
            case GameState.CREATE:
                MouseReset();
                inUseLayerMask = groundLayer;
                break;
            case GameState.MOVE:
            case GameState.DEMOLISH:
            case GameState.CUSTOMIZE:
                MouseReset();
                UpdatePrefabToBuild(null);
                inUseLayerMask = physicsLayer;
                break;
        }
    }

    /// <summary>
    /// Resets script options on creation and editing
    /// </summary>
    private void MouseReset()
    {
        isCreating = false;
        isBeingEdited = null;
    }

    /// <summary>
    /// Updates current prefab gameObject and type to be built
    /// </summary>
    /// <param name="p"></param>
    public void UpdatePrefabToBuild(GameObject p)
    {
        prefabModule = p;

        if(p != null)
            currentModuleType = prefabModule.GetComponent<ModuleBehavior>().MyType;
    }
}