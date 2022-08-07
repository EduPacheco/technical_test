using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region SINGLETON
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region VARIABLES

    [SerializeField] private GameObject creationPanel;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject windowPrefab;

    #endregion

    private void Start()
    {
        OnClick_CreationMode();
    }

    public void OnClick_CreationMode()
    {
        MouseTarget.instance.UpdateMouseTargetFunctionality(GameState.CREATE);

        creationPanel.SetActive(true);
    }

    public void OnClick_DemolishMode()
    {
        MouseTarget.instance.UpdateMouseTargetFunctionality(GameState.DEMOLISH);

        creationPanel.SetActive(false);
    }

    public void OnClick_CustomizeMode()
    {
        MouseTarget.instance.UpdateMouseTargetFunctionality(GameState.CUSTOMIZE);

        creationPanel.SetActive(false);
    }

    public void OnClick_SelectWallToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(wallPrefab);
    }

    public void OnClick_SelectFloorToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(floorPrefab);
    }

    public void OnClick_SelectDoorToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(doorPrefab);
    }

    public void OnClick_SelectWindowToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(windowPrefab);
    }
}
