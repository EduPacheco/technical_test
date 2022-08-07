using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References: https://youtu.be/0jTPKz3ga4w
/// </summary>

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
    [SerializeField] private float nodeSize = .5f;
    [SerializeField] private Camera mainCam;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask physicsLayer;

    private LayerMask inUseLayerMask;

    private GameObject prefabBuilding;
    [SerializeField] private GameObject[] pointers;
    private GameObject currentPointer;


    private bool isCreating = false;

    //private GameObject currentBuilding;


    private Vector3 previousPosition;

    private List<GameObject> creationBuffer = new List<GameObject>();

    #endregion

    private void Start()
    {
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
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, inUseLayerMask))
        {
            //Show creation Pointer
            if (currentPointer != null && !currentPointer.activeSelf)
            {
                currentPointer.SetActive(true);
            }

            //Snapping to grid Position without grid info
            Vector3 t = hit.point;

            switch (UIManager.instance.CurrentState)
            {
                case GameState.CREATE:
                    //Clear any module created on previous creation click
                    CreationMode(t);
                    break;
                case GameState.DEMOLISH:
                    DemolishMode(hit);
                    break;
                case GameState.CUSTOMIZE:
                    CustomizeMode(hit);
                    break;
            }      
        }
        else
        {
            if (currentPointer != null && currentPointer.activeSelf)
            {
                currentPointer.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(1))//Mouse Right Click
        {
            switch (UIManager.instance.CurrentState)
            {
                case GameState.CREATE:
                    //Clear any module created on previous creation click
                    ClearCreationBuffer();
                    break;
            }
        }
    }

    private void CreationMode(Vector3 target)
    {
        Vector3 snappingGrid = new Vector3(
                    Mathf.RoundToInt(target.x / nodeSize) * nodeSize,
                    0f,
                    Mathf.RoundToInt(target.z / nodeSize) * nodeSize);

        transform.position = snappingGrid;

        if (snappingGrid != previousPosition)
        {
            if (isCreating)
                CreateModule(snappingGrid);

            previousPosition = snappingGrid;
        }

        if (Input.GetMouseButtonDown(0))//Mouse Left Click
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

            isCreating = !isCreating;
        }
    }

    private void ClearCreationBuffer()
    {
        if (isCreating)
        {
            if (creationBuffer.Count != 0)
            {
                foreach (GameObject g in creationBuffer)
                {
                    Destroy(g);
                }

                creationBuffer.Clear();
            }

            isCreating = false;
        }
    }

    private void DemolishMode(RaycastHit target)
    {
        transform.position = target.point;

        if (Input.GetMouseButtonDown(0))//Mouse Left Click
        {
            ModuleBehavior targetModule = target.transform.gameObject.GetComponent<ModuleBehavior>();
            if(targetModule)
                Destroy(targetModule.gameObject);
        }
    }

    private void CustomizeMode(RaycastHit target)
    {
        transform.position = target.point;

        ModuleBehavior targetModule = target.transform.gameObject.GetComponent<ModuleBehavior>();

        if (!targetModule)
            return;

        if (Input.GetMouseButtonDown(0))//Mouse Left Click
            targetModule.ChangeVisuals(true);
        if (Input.GetMouseButton(0))//Mouse Left Click
        if (Input.GetMouseButtonDown(1))//Mouse Right Click
            targetModule.ChangeVisuals(false);
    }

    private void CreateModule(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - previousPosition).normalized;
        GameObject module = Instantiate(prefabBuilding, previousPosition, Quaternion.identity, null);

        module.transform.forward = dir;

        creationBuffer.Add(module);
    }

    public void UpdateMouseTargetFunctionality()
    {
        //Change user mouse pointer
        if(currentPointer != null)
            currentPointer.SetActive(false);
        currentPointer = pointers[(int)UIManager.instance.CurrentState];
        currentPointer.SetActive(true);

        //Change Layer Mask Raycast Target
        switch (UIManager.instance.CurrentState)
        {
            case GameState.CREATE:
                inUseLayerMask = groundLayer;
                break;
            case GameState.DEMOLISH:
            case GameState.CUSTOMIZE:
                inUseLayerMask = physicsLayer;
                break;
        }
    }

    public void UpdatePrefabToBuild(GameObject p)
    {
        prefabBuilding = p;
    }
}