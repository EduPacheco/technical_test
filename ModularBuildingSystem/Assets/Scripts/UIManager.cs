using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Text createText;
    [SerializeField] private Text moveText;
    [SerializeField] private Text customizeText;
    [SerializeField] private Text demolishText;

    #endregion

    private void Start()
    {
        OnClick_CreationMode();
    }

    #region UI Management Methods

    private void UpdateStateAndUI(GameState nextState)
    {
        creationPanel.SetActive(false);

        MouseTarget.instance.UpdateMouseTargetFunctionality(nextState);

        if (createText.enabled)
            createText.enabled = false;
        if (moveText.enabled)
            moveText.enabled = false;
        if (customizeText.enabled)
            customizeText.enabled = false;
        if (demolishText.enabled)
            demolishText.enabled = false;

        switch(nextState)
        {
            case GameState.CREATE:
                createText.enabled = true;
                creationPanel.SetActive(true);
                break;

            case GameState.MOVE:
                moveText.enabled = true;
                break;

            case GameState.CUSTOMIZE:
                customizeText.enabled = true;
                break;

            case GameState.DEMOLISH:
                demolishText.enabled = true;
                break;
        }
    }

    #endregion

    #region OnClick_ButtonMethods
    public void OnClick_CreationMode()
    {
        UpdateStateAndUI(GameState.CREATE);      
    }

    public void OnClick_MoveMode()
    {
        UpdateStateAndUI(GameState.MOVE); 
    }

    public void OnClick_CustomizeMode()
    {
        UpdateStateAndUI(GameState.CUSTOMIZE);
    }

    public void OnClick_DemolishMode()
    {
        UpdateStateAndUI(GameState.DEMOLISH);
    }

    public void OnClick_ExitApplication()
    {
        Application.Quit();
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