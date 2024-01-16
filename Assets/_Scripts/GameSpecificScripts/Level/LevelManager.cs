using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [Header("DEPENDENCIES")]
    public GameObject levelEndPrefab;
    public int levelIndex;
    public LevelData[] levels;
    public LevelData[] levels2;

    [HideInInspector]
    public bool levelEndSpawned = false;

    private GridManager gridManager;
    private LevelData currentLevel;
    private FatmanAnimationController[] fatmanAnimationController;

    public GameObject walkableTilesParent;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitLevel();

        fatmanAnimationController = FindObjectsOfType<FatmanAnimationController>();
    }

    public void InitLevel()
    {
        gridManager = FindObjectOfType<GridManager>();

        if (GameManager.instance.currentLevel <= 45)
        {
            levelIndex = (GameManager.instance.currentLevel - 1) % levels.Length;
            currentLevel = levels[levelIndex];
            gridManager.InitGrid(levels);
        }
        else if (GameManager.instance.currentLevel > 45)
        {
            levelIndex = (GameManager.instance.currentLevel - 1) % levels2.Length;
            currentLevel = levels2[levelIndex];
            gridManager.InitGrid(levels2);
        }

        InitWalkableTiles();

        InitStaticTiles();

        InitObstacleTiles();

        InitEnviroment();

        InitCamera();

        if (currentLevel.canPlayTutorial)
        {
            //Play hand tutorial

            var staticTiles = FindObjectsOfType<StaticTileController>();

            for (int i = 0; i < staticTiles.Length; i++)
            {
                staticTiles[i].tutorialCanvas.gameObject.SetActive(true);
                staticTiles[i].DOTutorialHandMove(currentLevel, (staticTiles.Length - 1) - i);
            }
        }
    }

    public void InitWalkableTiles()
    {
        walkableTilesParent = new GameObject("Walkable Tiles");

        for (int i = 0; i < currentLevel.walkableTiles.Count; i++)
        {
            var walkableTileObj = currentLevel.walkableTiles[i].tileObj;
            var gridIndex = currentLevel.walkableTiles[i].gridIndex;

            Vector3 worldPos = gridManager.GridToWorld(gridManager.virtualGrid[gridIndex].x, gridManager.virtualGrid[gridIndex].z);

            var go = Instantiate(walkableTileObj, worldPos, Quaternion.identity, walkableTilesParent.transform);

            gridManager.virtualGrid[gridIndex].SetGameObject(go);
        }
    }

    public void InitObstacleTiles()
    {
        var obstacleTiles = new GameObject("Obstacle Tiles");
        for (int i = 0; i < currentLevel.obstacleTiles.Count; i++)
        {
            var obstacleTileObj = currentLevel.obstacleTiles[i].tileObj;
            var gridIndex = currentLevel.obstacleTiles[i].gridIndex;

            Vector3 worldPos = gridManager.GridToWorld(gridManager.virtualGrid[gridIndex].x, gridManager.virtualGrid[gridIndex].z);

            var go = Instantiate(obstacleTileObj, worldPos, Quaternion.identity, obstacleTiles.transform);

            gridManager.virtualGrid[gridIndex].SetGameObject(go);
        }
    }

    public void InitStaticTiles()
    {
        var staticTilesParent = new GameObject("Static Tiles");

        var stickMenParent = new GameObject("Stick Men");

        var staticTileData = currentLevel.staticTiles;

        for (int i = 0; i < staticTileData.Count; i++)
        {
            var staticTileObj = staticTileData[i].tileObj;
            var gridIndex = staticTileData[i].gridIndex;
            var stickManCount = staticTileData[i].stickManCount;

            Vector3 worldPos = gridManager.GridToWorld(gridManager.virtualGrid[gridIndex].x, gridManager.virtualGrid[gridIndex].z);

            var go = Instantiate(staticTileObj, worldPos, Quaternion.identity, staticTilesParent.transform);

            gridManager.virtualGrid[gridIndex].SetGameObject(go);

            // var currentStaticTile = go.GetComponent<StaticTileController>();
            var currentStaticTile = go.GetComponent<TileController>();

            currentStaticTile.GetComponentInChildren<SpawnPoints>().InitSpawnPoints(stickManCount);
            for (int j = 0; j < stickManCount; j++)
            {

                var stickManGO = Instantiate(currentStaticTile.stickManPrefab, currentStaticTile.stickManSpawnPoints[j].position, currentStaticTile.stickManPrefab.transform.rotation, stickMenParent.transform);

                stickManGO.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = currentLevel.staticTiles[i].stickManColor;
                // stickManGO.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color =
                //     new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                stickManGO.GetComponent<StickMan>().SetNumber(j + 1);
                // stickManGO.GetComponent<StickMan>().currentTile = currentStaticTile.virtualGrid;

                if (j == stickManCount - 1)
                {
                    stickManGO.GetComponent<StickMan>().SetCount(j);
                }

                // currentStaticTile.stickMen.Add(stickManGO);
                currentStaticTile.stickMen.Add(stickManGO);
                currentStaticTile.stickMenAtStatic.Add(stickManGO);
                // currentStaticTile.totalStickManCount = stickManCount;
                // currentStaticTile.currentStickManCount = currentStaticTile.totalStickManCount;
                currentStaticTile.SetColor(staticTileData[i].stickManColor);
            }
        }
    }

    public void InitEnviroment()
    {
        currentLevel.enviromentData.SetFrame();
        currentLevel.enviromentData.SetCircleGround();
        currentLevel.enviromentData.SetDoors();
    }

    public void InitCamera()
    {
        currentLevel.cameraData.SetCam();
    }

    public bool CheckForCompletion()
    {
        for (int i = 0; i < walkableTilesParent.transform.childCount; i++)
        {
            if (walkableTilesParent.transform.GetChild(i).GetComponent<TileController>().isOccupied == false)
            {
                // Debug.LogError("Not Complete");
                return false;
            }
        }
        SpawnLevelEnd();
        levelEndSpawned = true;
        return true;
    }

    public void SpawnLevelEnd()
    {
        var gridFrame = FindObjectOfType<GridFrameController>();
        gridFrame.ChangeColor();

        var obstacleTiles = FindObjectsOfType<ObstacleTileController>();

        foreach (var obstacleTile in obstacleTiles)
        {
            obstacleTile.HandleForLevelEnd();
        }

        Instantiate(levelEndPrefab, Vector3.zero, Quaternion.identity);

        foreach (var anim in fatmanAnimationController)
        {
            anim.RandomDanceAnims();
        }

        StickMan[] stickMenAll = FindObjectsOfType<StickMan>();

        for (int i = 0; i < stickMenAll.Length; i++)
        {
            stickMenAll[i].numberParent.gameObject.SetActive(false);
        }

        GameManager.instance.LevelComplete();
    }
}
