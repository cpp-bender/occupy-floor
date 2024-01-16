using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class StaticTileController : BaseTileController
{
    [Header("DEPENDENCIES"), Space(5f)]
    public GameObject stickManPrefab;
    public GameObject button;
    public TutorialCanvasManager tutorialCanvas;

    [Header("DEBUG"), Space(5f)]
    public List<GameObject> stickMen;
    public List<WalkableTileController> possibleWalkableTiles;

    public int totalStickManCount;
    public int currentStickManCount;
    public int onTileStickManCount;
    public List<Transform> stickManSpawnPoints;
    public VirtualGrid virtualGrid;

    public List<VirtualGrid> tilesOccupied = new List<VirtualGrid>();
    public List<VirtualGrid> tilesNotOccupied;
    public List<VirtualGrid> tilesSaved;
    public List<VirtualGrid> tilesOccupiedInOrder = new List<VirtualGrid>();

    private VirtualGrid tilePressed;

    public enum Direction { right, left, top, down }

    public Direction currentDirection;

    private Sequence moveSeq;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        possibleWalkableTiles = GridManager.Instance.GetPossibleWalkableTiles(transform.position);

        virtualGrid = GridManager.Instance.WorldToGrid(transform.position);
    }

    public void DOTutorialHandMove(LevelData currentLevel, int index)
    {
        var hand = tutorialCanvas.hand;

        hand.transform.position = Camera.main.WorldToScreenPoint(transform.position);

        moveSeq = DOTween.Sequence();

        var staticTileData = currentLevel.staticTiles;

        for (int i = 0; i < currentLevel.staticTiles[index].handTutorialTweens.Length; i++)
        {
            moveSeq.Append(staticTileData[index].handTutorialTweens[i].GetTween(hand));
        }

        moveSeq.Play().SetLoops(-1, LoopType.Restart)
            .SetId("handtutorialtween")
            .OnKill(delegate
            {
                Destroy(hand);
            });
    }

    private void OnMouseDown()
    {
        // raycastManager.staticTile = this;
    }

    private void OnMouseUp()
    {
        // raycastManager.staticTile = null;
    }

    public void SetColor(Color color)
    {
        button.GetComponent<SkinnedMeshRenderer>().material.color = color;
    }

    public void ClearAvailableTiles()
    {
        tilesOccupied.Clear();

        AddSavedTiles();



        NonOccupyTile();

        DistributeStickMen();
    }

    private void NonOccupyTile()
    {
        tilesNotOccupied = new List<VirtualGrid>();

        for (int i = 0; i < GridManager.Instance.virtualGrid.Count; i++)
        {
            if (GridManager.Instance.virtualGrid[i].tileObj.GetComponent<WalkableTileController>() != null)
            {
                if (!tilesOccupied.Contains(GridManager.Instance.virtualGrid[i]))
                {
                    tilesNotOccupied.Add(GridManager.Instance.virtualGrid[i]);

                    if (possibleWalkableTiles.Contains(GridManager.Instance.virtualGrid[i].tileObj.GetComponent<WalkableTileController>()) && GridManager.Instance.virtualGrid[i].tileObj.GetComponent<WalkableTileController>().everOccupied)
                    {
                        // GridManager.Instance.virtualGrid[i].tileObj.GetComponent<WalkableTileController>().SetNumber(0);
                    }


                }

            }
        }

        for (int i = 0; i < GridManager.Instance.virtualGrid.Count; i++)
        {
            for (int j = 0; j < tilesOccupied.Count; j++)
            {
                tilesNotOccupied.Remove(tilesOccupied[j]);

                // tilesOccupied[j].tileObj.GetComponent<WalkableTileController>().SetNumber(0);
            }
        }
    }

    public void AddOccupiedTileInOrder(VirtualGrid virtualGridToOccupy)
    {
        if (!tilesOccupiedInOrder.Contains(virtualGridToOccupy))
        {
            tilesOccupiedInOrder.Add(virtualGridToOccupy);
            // virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().IncreaseNumber();
        }
        else
        {
            // virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().DecreaseNumber();
        }

    }

    public void RemoveOccupiedTileInOrder(VirtualGrid virtualGridToOccupy)
    {
        if (tilesOccupiedInOrder.Contains(virtualGridToOccupy))
        {
            tilesOccupiedInOrder.Remove(virtualGridToOccupy);
        }
    }

    public void FindAvailableTiles(VirtualGrid walkableTileVirtualGrid)
    {
        List<int[]> tempList = CheckNeighbours(walkableTileVirtualGrid);
        tilesOccupied = new List<VirtualGrid>();

        AddSavedTiles();  // #1 Saved tiles are added

        for (int i = 0; i < GridManager.Instance.virtualGrid.Count; i++)
        {
            for (int j = 0; j < tempList.Count; j++)
            {
                if (GridManager.Instance.virtualGrid[i].x == tempList[j][0] && GridManager.Instance.virtualGrid[i].z == tempList[j][1])
                {
                    // Debug.LogError("Occupy1");
                    OccupyTile(GridManager.Instance.virtualGrid[i]); // #3 Tiles between pressed and static are added
                }
            }
        }

        if (tempList.Count == 0)
        {
            tilePressed = walkableTileVirtualGrid;
        }
        else
        {
            if (currentDirection == Direction.top)
            {
                if (walkableTileVirtualGrid.z == tempList[tempList.Count - 1][1] + 1)
                {
                    tilePressed = walkableTileVirtualGrid;
                }
            }
            else if (currentDirection == Direction.down)
            {
                if (walkableTileVirtualGrid.z == tempList[tempList.Count - 1][1] - 1)
                {
                    tilePressed = walkableTileVirtualGrid;
                }
            }
            else if (currentDirection == Direction.right)
            {
                if (walkableTileVirtualGrid.x == tempList[tempList.Count - 1][0] + 1)
                {
                    tilePressed = walkableTileVirtualGrid;
                }
            }
            else if (currentDirection == Direction.left)
            {
                if (walkableTileVirtualGrid.x == tempList[tempList.Count - 1][0] - 1)
                {
                    tilePressed = walkableTileVirtualGrid;
                }
            }
        }

        // Debug.LogError("Occupy2");
        OccupyTile(tilePressed); // #2 Pressed tile are added



        NonOccupyTile();

        DistributeStickMen();
    }

    private void AddSavedTiles()
    {
        for (int i = 0; i < tilesSaved.Count; i++)
        {
            if (!tilesOccupied.Contains(tilesSaved[i]))
            {
                tilesOccupied.Add(tilesSaved[i]);
            }
        }
    }

    public void RemoveOccupiedTile(VirtualGrid virtualGridToRemove)
    {
        if (tilesOccupied.Contains(virtualGridToRemove))
        {
            tilesOccupied.Remove(virtualGridToRemove);
            RemoveOccupiedTileInOrder(virtualGridToRemove);
            // Debug.LogError("Here");
            // virtualGridToRemove.tileObj.GetComponent<WalkableTileController>().IncreaseNumber();
            UnSaveTile(virtualGridToRemove);
            NonOccupyTile();
            DistributeStickMen();
        }
    }

    public void UnSaveTile(VirtualGrid virtualGridToRemove)
    {
        if (tilesSaved.Contains(virtualGridToRemove))
        {
            tilesSaved.Remove(virtualGridToRemove);
        }
    }

    public void SaveTile()
    {
        for (int i = 0; i < tilesOccupied.Count; i++)
        {
            if (!tilesSaved.Contains(tilesOccupied[i]))
            {
                tilesSaved.Add(tilesOccupied[i]);

                // AssignStaticTileToWalkableTile(tilesOccupied[i].tileObj.GetComponent<WalkableTileController>(), this);
            }
        }
    }

    private void AssignStaticTileToWalkableTile(WalkableTileController walkableTileController, StaticTileController staticTileController)
    {
        walkableTileController.staticTile = staticTileController;

        if (staticTileController != null)
        {
            walkableTileController.direction = staticTileController.currentDirection;
        }

    }

    public void DistributeStickMen()
    {
        // Debug.LogError("Dist");

        for (int i = 0; i < tilesOccupied.Count; i++)
        {
            for (int j = 0; j < stickMen.Count; j++)
            {
                if (stickMen[j].GetComponent<StickMan>().number ==
                    tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().number)
                {
                    stickMen[j].transform.DOMove(tilesOccupied[i].tileObj.transform.position + Vector3.up * 0.1f, 0.1f).Play();
                    // Debug.LogError("Move");
                }
            }

            // stickMen[i].transform.position = tilesOccupied[i].tileObj.transform.position + Vector3.up * 0.1f;
            // stickMen[i].transform.DOMove(tilesOccupied[i].tileObj.transform.position + Vector3.up * 0.1f, 0.1f).Play();
        }

        for (int i = tilesOccupied.Count; i < stickMen.Count; i++)
        {
            // stickMen[i].transform.position = stickManSpawnPoints[i].position;
            stickMen[i].transform.DOMove(stickManSpawnPoints[i].position, 0.1f).Play();
        }

        currentStickManCount = totalStickManCount - tilesOccupied.Count;
    }

    private void OccupyTile(VirtualGrid virtualGridToOccupy)
    {
        if (tilesOccupied.Count < (totalStickManCount - 1))
        {
            if (!tilesOccupied.Contains(virtualGridToOccupy))
            {
                tilesOccupied.Add(virtualGridToOccupy);
                AssignStaticTileToWalkableTile(virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>(), this);

                virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().FindTilesAtTheSameDirection();



                // int number = tilesOccupied.Count - FindNumber(virtualGridToOccupy) + 1;
                int number = FindNumber(virtualGridToOccupy);

                // Debug.LogError(tilesOccupied.Count + " " + FindNumber(virtualGridToOccupy));

                virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().everOccupied = true;
                // virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().SetNumber(number + tilesSaved.Count);

                AddOccupiedTileInOrder(virtualGridToOccupy);
                // virtualGridToOccupy.tileObj.GetComponent<WalkableTileController>().IncreaseNumber(tilesSaved.Count);
            }
        }
    }

    private List<int[]> CheckNeighbours(VirtualGrid walkableTileVirtualGrid)
    {
        int sx = virtualGrid.x;
        int sz = virtualGrid.z;

        int wx = walkableTileVirtualGrid.x;
        int wz = walkableTileVirtualGrid.z;

        int[] tempGridValues = new int[4];
        List<int> listOfTilesBetweenGridValuesOriginal = new List<int>();

        int counter = 0;

        List<int[]> tilesBetweenGrids = new List<int[]>();

        if (sx == wx) // top-down
        {
            if (sz < wz) // top
            {
                listOfTilesBetweenGridValuesOriginal = FindTilesBetween(sz, wz);

                for (int j = 0; j < listOfTilesBetweenGridValuesOriginal.Count; j++)
                {
                    int[] tempGrid = new int[2];
                    tempGrid[0] = sx;
                    tempGrid[1] = listOfTilesBetweenGridValuesOriginal[j];

                    tilesBetweenGrids.Add(tempGrid);
                }

                currentDirection = Direction.top;
            }
            else if (sz > wz) // down
            {
                listOfTilesBetweenGridValuesOriginal = FindTilesBetween(sz, wz);

                for (int j = 0; j < listOfTilesBetweenGridValuesOriginal.Count; j++)
                {
                    int[] tempGrid = new int[2];
                    tempGrid[0] = sx;
                    tempGrid[1] = listOfTilesBetweenGridValuesOriginal[j];

                    tilesBetweenGrids.Add(tempGrid);
                }

                currentDirection = Direction.down;
            }
        }
        else if (sz == wz) // right-left
        {
            if (sx < wx) // right
            {
                listOfTilesBetweenGridValuesOriginal = FindTilesBetween(sx, wx);

                for (int j = 0; j < listOfTilesBetweenGridValuesOriginal.Count; j++)
                {
                    int[] tempGrid = new int[2];
                    tempGrid[1] = sz;
                    tempGrid[0] = listOfTilesBetweenGridValuesOriginal[j];

                    tilesBetweenGrids.Add(tempGrid);
                }

                currentDirection = Direction.right;
            }
            else if (sx > wx) // left
            {
                listOfTilesBetweenGridValuesOriginal = FindTilesBetween(sx, wx);

                for (int j = 0; j < listOfTilesBetweenGridValuesOriginal.Count; j++)
                {
                    int[] tempGrid = new int[2];
                    tempGrid[1] = sz;
                    tempGrid[0] = listOfTilesBetweenGridValuesOriginal[j];

                    tilesBetweenGrids.Add(tempGrid);
                }

                currentDirection = Direction.left;
            }
        }
        return tilesBetweenGrids;
    }

    private List<int> FindTilesBetween(int start, int end)
    {
        List<int> listOfTilesBetweenGridValues = new List<int>();
        int dif = end - start;
        int increment;

        if (dif > 0)
        {
            increment = Mathf.Clamp(dif - 1, 0, currentStickManCount);

            for (int i = 0; i < increment; i++)
            {
                listOfTilesBetweenGridValues.Add(start + (i + 1));
            }
        }
        else if (dif < 0)
        {
            increment = Mathf.Clamp(Mathf.Abs(dif) - 1, 0, currentStickManCount);

            for (int i = 0; i < increment; i++)
            {
                listOfTilesBetweenGridValues.Add(start - (i + 1));
            }
        }

        return listOfTilesBetweenGridValues;
    }

    private int FindNumber(VirtualGrid virtualGridToNumber)
    {
        int sx = virtualGrid.x;
        int sz = virtualGrid.z;

        int wx = virtualGridToNumber.x;
        int wz = virtualGridToNumber.z;

        Direction direction = virtualGridToNumber.tileObj.GetComponent<WalkableTileController>().direction;

        int number = 0;

        if (direction == Direction.top)
        {
            number = Mathf.Abs(sz - wz);
        }
        else if (direction == Direction.down)
        {
            number = Mathf.Abs(sz - wz);
        }
        else if (direction == Direction.right)
        {
            number = Mathf.Abs(sx - wx);
        }
        else if (direction == Direction.left)
        {
            number = Mathf.Abs(sx - wx);
        }

        return number;
    }
}