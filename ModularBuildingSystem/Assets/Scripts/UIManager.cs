using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    CREATE,
    DEMOLISH,
    CUSTOMIZE
}

public enum ModuleType
{
    WALL,
    FLOOR,
    DOOR,
    WINDOW
}

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

    private GameState currentState;
    public GameState CurrentState { get => currentState; }

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
        currentState = GameState.CREATE;
        MouseTarget.instance.UpdateMouseTargetFunctionality();

        creationPanel.SetActive(true);
    }

    public void OnClick_DemolishMode()
    {
        currentState = GameState.DEMOLISH;
        MouseTarget.instance.UpdateMouseTargetFunctionality();

        creationPanel.SetActive(false);
    }

    public void OnClick_CustomizeMode()
    {
        currentState = GameState.CUSTOMIZE;
        MouseTarget.instance.UpdateMouseTargetFunctionality();

        creationPanel.SetActive(false);
    }

    public void OnClick_SelectWallToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(wallPrefab, ModuleType.WALL);
    }

    public void OnClick_SelectFloorToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(floorPrefab, ModuleType.FLOOR);
    }

    public void OnClick_SelectDoorToBuild()
    {
        MouseTarget.instance.UpdatePrefabToBuild(doorPrefab, ModuleType.DOOR);
    }
}
