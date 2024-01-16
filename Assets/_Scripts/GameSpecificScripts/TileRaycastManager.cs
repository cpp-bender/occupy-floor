using UnityEngine;

public class TileRaycastManager : SingletonMonoBehaviour<TileRaycastManager>
{
    public TileController startTile;
    public TileController startStaticTile;
    public TileController startWalkableTile;
    public TileController startObstacleTile;
    public TileController currentTile;
    public TileController previousTile;

    
    protected override void Awake()
    {
        base.Awake();
        

    }

    private void Update()
    {
        if (GameManager.instance.isLevelCompleted)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                GameObject hitObject = raycastHit.collider.gameObject;

                startTile = hitObject.GetComponent<TileController>();

                if (startTile != null)
                {
                    if (startTile.tileType == TileController.TileType.Static)
                    {
                        startStaticTile = startTile;
                        // Debug.LogError("Start static");
                    }
                    else if (startTile.tileType == TileController.TileType.Walkable)
                    {
                        startWalkableTile = startTile;
                        // Debug.LogError("Start walkable");
                    }
                    else if (startTile.tileType == TileController.TileType.Obstacle)
                    {
                        startObstacleTile = startTile;
                        // Debug.LogError("Start obstacle");
                    }
                }

            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.black);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                GameObject hitObject = raycastHit.collider.gameObject;

                currentTile = hitObject.GetComponent<TileController>();

                if (currentTile != previousTile)
                {
                    if (previousTile != null)
                    {
                        if (previousTile.tileType == TileController.TileType.Static)
                        {
                            if (currentTile.tileType == TileController.TileType.Static)
                            {
                                // Debug.LogError("Continue static");
                            }
                            else if (currentTile.tileType == TileController.TileType.Walkable)
                            {
                                
                                if (startStaticTile != null)
                                {
                                    startStaticTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                else
                                {
                                    previousTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                
                                // Debug.LogError("Continue walkable");
                            }
                            else if (currentTile.tileType == TileController.TileType.Obstacle)
                            {
                                // Debug.LogError("Continue obstacle");
                            }
                        }
                        else if (previousTile.tileType == TileController.TileType.Walkable)
                        {
                            if (currentTile.tileType == TileController.TileType.Static)
                            {
                                if (startStaticTile != null)
                                {
                                    startStaticTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                else
                                {
                                    previousTile.parentStaticTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                
                                // Debug.LogError("Continue static - return home");
                            }
                            else if (currentTile.tileType == TileController.TileType.Walkable)
                            {
                                if (startStaticTile != null)
                                {
                                    startStaticTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                else
                                {
                                    previousTile.parentStaticTile.Occupy(previousTile.virtualGrid, currentTile.virtualGrid);
                                }
                                
                                
                                // Debug.LogError("Continue walkable");
                            }
                            else if (currentTile.tileType == TileController.TileType.Obstacle)
                            {
                                // Debug.LogError("Continue obstacle");
                            }
                        }
                        else if (previousTile.tileType == TileController.TileType.Obstacle)
                        {
                            if (currentTile.tileType == TileController.TileType.Static)
                            {
                                // Debug.LogError("Continue static");
                            }
                            else if (currentTile.tileType == TileController.TileType.Walkable)
                            {
                                // Debug.LogError("Continue walkable");
                            }
                            else if (currentTile.tileType == TileController.TileType.Obstacle)
                            {
                                // Debug.LogError("Continue obstacle");
                            }
                        }
                    }
                    
                    previousTile = currentTile;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentTile != null)
            {
                if (currentTile.tileType == TileController.TileType.Static)
                {
                    // Debug.LogError("End static");
                }
                else if (currentTile.tileType == TileController.TileType.Walkable)
                {
                    // Debug.LogError("End walkable");
                }
                else if (currentTile.tileType == TileController.TileType.Obstacle)
                {
                    // Debug.LogError("End obstacle");
                }
            }

            startTile = null;
            currentTile = null;
            previousTile = null;
            startWalkableTile = null;
            startStaticTile = null;
            startObstacleTile = null;
        }
        
        if (!LevelManager.Instance.levelEndSpawned)
        {
            LevelManager.Instance.CheckForCompletion();
        }
    }
}
