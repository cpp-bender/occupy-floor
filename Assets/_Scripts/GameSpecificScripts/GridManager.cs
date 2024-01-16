using System.Collections.Generic;
using UnityEngine;

public class GridManager : SingletonMonoBehaviour<GridManager>
{
    [Header("SELF DEPENDENCIES"), Space(5f)]
    public GameObject virtualGridObj;

    [Header("DEBUG"), Space(5f)]
    public List<VirtualGrid> virtualGrid;
    public List<Vector3> gridWorldPos;

    private List<int> neighboursX;
    private List<int> neighboursZ;

    protected override void Awake()
    {
        base.Awake();
    }

    private void InitNeighboursIndexes(int width, int height)
    {
        neighboursX = new List<int>();

        neighboursZ = new List<int>();

        //Left
        for (int i = 1; i < width; i++)
        {
            neighboursX.Add(-1 * i);
            neighboursZ.Add(0);
        }

        //Right
        for (int i = 1; i < width; i++)
        {
            neighboursX.Add(1 * i);
            neighboursZ.Add(0);
        }

        //Up
        for (int i = 1; i < height; i++)
        {
            neighboursZ.Add(1 * i);
            neighboursX.Add(0);
        }

        //Down
        for (int i = 1; i < height; i++)
        {
            neighboursZ.Add(-1 * i);
            neighboursX.Add(0);
        }
    }

    public void InitGrid(LevelData[] levels)
    {
        virtualGrid = new List<VirtualGrid>();
        gridWorldPos = new List<Vector3>();

        var levelManager = LevelManager.Instance;

        var currentLevel = levels[levelManager.levelIndex];

        InitNeighboursIndexes(currentLevel.width, currentLevel.height);

        Vector3 worldPos = Vector3.zero;

        Vector3 initialPos = currentLevel.initialPos;

        for (int z = 0; z < currentLevel.height; z++)
        {
            for (int x = 0; x < currentLevel.width; x++)
            {
                worldPos = initialPos + new Vector3(x * currentLevel.widthThreshold, 0f, z * currentLevel.heightThreshold);
                virtualGrid.Add(new VirtualGrid(x, z));
                gridWorldPos.Add(worldPos);
            }
        }
    }

    public Vector3 GridToWorld(int x, int z)
    {
        for (int i = 0; i < virtualGrid.Count; i++)
        {
            if (virtualGrid[i].x == x && virtualGrid[i].z == z)
            {
                return gridWorldPos[i];
            }
        }
        return Vector3.zero;
    }

    public VirtualGrid WorldToGrid(Vector3 worlPos)
    {
        for (int i = 0; i < gridWorldPos.Count; i++)
        {
            if (gridWorldPos[i] == worlPos)
            {
                return virtualGrid[i];
            }
        }
        return null;
    }

    public List<WalkableTileController> GetPossibleWalkableTiles(Vector3 worlPos)
    {
        VirtualGrid grid = WorldToGrid(worlPos);

        var tempList = new List<WalkableTileController>();

        int maxNeihbourCount = neighboursX.Count;

        int x = grid.x;
        int z = grid.z;

        for (int i = 0; i < maxNeihbourCount; i++)
        {
            int nx = x + neighboursX[i];
            int nz = z + neighboursZ[i];
            for (int k = 0; k < virtualGrid.Count; k++)
            {
                if (virtualGrid[k].x == nx && virtualGrid[k].z == nz)
                {
                    if (virtualGrid[k].tileObj.CompareTag(Tags.WALKABLE))
                    {
                        tempList.Add(virtualGrid[k].tileObj.GetComponent<WalkableTileController>());
                    }
                }
            }
        }
        return tempList;
    }
}
