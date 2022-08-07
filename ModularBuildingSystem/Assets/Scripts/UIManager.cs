using UnityEngine;

/// <summary>
/// User Interface Manager, has On click methods called from the buttons in the UI,
/// comunicates with the Mouse Script to let it now which mode the game currently is, 
/// (Create, MOve, Customize, Demolish) and which module the player desires to build
/// </summary>


public class UIManager : MonoBehaviour
{
    #region VARIABLES

    /// <summary>
    /// UI Panel where are the module buttons choice
    /// </summary>
    [SerializeField] private GameObject creationPanel;

    /// <summary>
    /// Possible modules the user can build
    /// </summary>
    [Header("Modules")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject windowPrefab;

    #endregion

    private void Start()
    {
        OnClick_CreationMode();
    }

    #region OnClick_ButtonMethods
    public void OnClick_CreationMode()
    {
        MouseTarget.instance.UpdateMouseTargetFunctionality(GameState.CREATE);

        creationPanel.SetActive(true);
    }

    public void OnClick_MoveMode()
    {
        MouseTarget.instance.UpdateMouseTargetFunctionality(GameState.MOVE);

        creationPanel.SetActive(false);
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
    #endregion
}