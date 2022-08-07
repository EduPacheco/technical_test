using UnityEngine;

/// <summary>
/// Handles building module creation
/// 
/// </summary>

public class GridLayoutScr : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] private GameObject gridNode;

    [SerializeField] private int citySize = 10; //Public Debug

    private float gridOffset = .5f;

    private GameObject[,] cityGrid;

    #endregion

    private void Awake()
    {
        cityGrid = new GameObject[citySize, citySize];

        int cityOffset = citySize / 2;

        for(int i = 0; i < citySize; i++)
        {
            for(int j = 0; j < citySize; j++)
            {
                Vector3 newNodePos = new Vector3(
                    j - cityOffset,// + gridOffset, 
                    0f,
                    i - cityOffset);// + gridOffset);

                GameObject go = Instantiate(gridNode, newNodePos, Quaternion.identity, this.transform);

                cityGrid[j, i] = go;
            }
        }
    }

}