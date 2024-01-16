using UnityEngine;

public class RaycastManager : SingletonMonoBehaviour<RaycastManager>
{
    [Header("DEPENDENCIES")]
    public StaticTileController staticTile;
    public WalkableTileController walkableTile;
    private VirtualGrid lastVirtualGrid;

    public WalkableTileController currentWalkableTile;
    public WalkableTileController previousWalkableTile;
    
    public StaticTileController currentStaticTile;
    public StaticTileController previousStaticTile;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                GameObject hitObject = raycastHit.collider.gameObject;

                if (hitObject.CompareTag(Tags.WALKABLE))  // Walkable
                {
                    if (staticTile == null)
                    {
                        staticTile = walkableTile.staticTile;
                    }
                    
                    currentWalkableTile = hitObject.GetComponent<WalkableTileController>();

                    if (staticTile != null)
                    {
                        if (currentWalkableTile != previousWalkableTile)
                        {
                            for (int i = 0; i < staticTile.possibleWalkableTiles.Count; i++)
                            {
                                if (staticTile.possibleWalkableTiles[i] == currentWalkableTile)
                                {
                                    previousWalkableTile = currentWalkableTile;
                                    FindAvailableTiles(currentWalkableTile.virtualGrid);
                                    
                                    // staticTile.DetectDirection(currentWalkableTile.virtualGrid); // Static to walkable

                                    if (walkableTile != null)
                                    {
                                        currentWalkableTile.RemoveOccupiedTiles();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (hitObject.CompareTag(Tags.STATIC)) // Static
                {
                    currentStaticTile = hitObject.GetComponent<StaticTileController>();

                    if (currentStaticTile != previousStaticTile)
                    {
                        if (staticTile != null)
                        {
                            staticTile.DistributeStickMen();
                        }
                        
                        previousStaticTile = currentStaticTile;
                    }
                    else
                    {
                        if (staticTile != null && previousWalkableTile != null)
                        {
                            staticTile.UnSaveTile(previousWalkableTile.virtualGrid);
                            staticTile.ClearAvailableTiles();
                            
                            previousWalkableTile = null;
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            previousWalkableTile = null;
            SaveTiles();
        }
    }

    private void FindAvailableTiles(VirtualGrid virtualGrid)
    {
        if (staticTile != null)
        {
            staticTile.FindAvailableTiles(virtualGrid);
        }
    }
    
    private void SaveTiles()
    {
        if (staticTile != null)
        {
            staticTile.SaveTile();
            staticTile = null;
        }
    }
}
