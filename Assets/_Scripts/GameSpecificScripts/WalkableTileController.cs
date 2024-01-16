using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class WalkableTileController : BaseTileController
{
    public GameObject currentStickMan;
    
    public VirtualGrid virtualGrid;

    public StaticTileController staticTile;
    public StaticTileController.Direction direction;

    public List<VirtualGrid> tilesToRemoveOccupy;
    public int number;
    public TextMeshProUGUI numberText;
    public List<VirtualGrid> tilesAtSameDir;
    public bool everOccupied;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        
        virtualGrid = GridManager.Instance.WorldToGrid(transform.position);
    }

    private void OnMouseDown()
    {
        // raycastManager.walkableTile = this;
    }

    private void OnMouseUp()
    {
        // raycastManager.walkableTile = null;
    }

    public void SetNumber(int num)
    {
        number = num;
        numberText.text = number.ToString();
    }

    public void IncreaseNumber(int savedCount)
    {
        // number += savedCount;
        number++;
        numberText.text = number.ToString();
    }
    
    public void DecreaseNumber()
    {
        number--;
        numberText.text = number.ToString();
    }
    
    
    
    public void RemoveOccupiedTiles()
    {
        List<VirtualGrid> tilesToRemoveOccupyLocal = FindTiles();

        for (int i = 0; i < tilesToRemoveOccupyLocal.Count; i++)
        {
            staticTile.RemoveOccupiedTile(tilesToRemoveOccupyLocal[i]);
        }
        
        
        
        // for (int i = 0; i < staticTile.tilesOccupied.Count; i++)
        // {
        //     if (staticTile.tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().number == number)
        //     {
        //         staticTile.RemoveOccupiedTile(staticTile.tilesOccupied[i]);
        //     }
        //     
        // }
        
        // staticTile.RemoveOccupiedTile(virtualGrid);
    }

    public List<VirtualGrid> FindTilesAtTheSameDirection()
    {
        // Debug.LogError("here");
        List<VirtualGrid> tilesAtSameDirection = new List<VirtualGrid>();
        
        
        if (direction == StaticTileController.Direction.top)
        {
            for (int i = 0; i < staticTile.tilesOccupied.Count; i++)
            {
                if (staticTile.tilesOccupied[i].x == virtualGrid.x && staticTile.tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().direction == direction)
                {
                    tilesAtSameDirection.Add(staticTile.tilesOccupied[i]);
                }
            }
        }
        else if (direction == StaticTileController.Direction.down)
        {
            for (int i = 0; i < staticTile.tilesOccupied.Count; i++)
            {
                if (staticTile.tilesOccupied[i].x == virtualGrid.x && staticTile.tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().direction == direction)
                {
                    tilesAtSameDirection.Add(staticTile.tilesOccupied[i]);
                }
            }
        }
        else if (direction == StaticTileController.Direction.right)
        {
            for (int i = 0; i < staticTile.tilesOccupied.Count; i++)
            {
                if (staticTile.tilesOccupied[i].z == virtualGrid.z && staticTile.tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().direction == direction)
                {
                    tilesAtSameDirection.Add(staticTile.tilesOccupied[i]);
                }
            }
        }
        else if (direction == StaticTileController.Direction.left)
        {
            for (int i = 0; i < staticTile.tilesOccupied.Count; i++)
            {
                if (staticTile.tilesOccupied[i].z == virtualGrid.z && staticTile.tilesOccupied[i].tileObj.GetComponent<WalkableTileController>().direction == direction)
                {
                    tilesAtSameDirection.Add(staticTile.tilesOccupied[i]);
                }
            }
        }

        tilesAtSameDir = new List<VirtualGrid>(tilesAtSameDirection);
        return tilesAtSameDirection;
    }
    
    private List<VirtualGrid> FindTiles()
    {
        List<int> listOfTilesBetweenGridValues = FindGridValuesOfTilesBetweenStaticTile();
        List<VirtualGrid> tilesAtSameDirection = FindTilesAtTheSameDirection();
        tilesAtSameDir = new List<VirtualGrid>(tilesAtSameDirection);
        
        if (direction == StaticTileController.Direction.top)
        {
            tilesToRemoveOccupy = new List<VirtualGrid>(tilesAtSameDirection);
            
            for (int i = 0; i < tilesAtSameDirection.Count; i++)
            {
                for (int j = 0; j < listOfTilesBetweenGridValues.Count; j++)
                {
                    if (tilesAtSameDirection[i].z == listOfTilesBetweenGridValues[j] && tilesToRemoveOccupy.Contains(tilesAtSameDirection[i]))
                    {
                        tilesToRemoveOccupy.Remove(tilesAtSameDirection[i]);
                    }
                }
            }
        }
        else if (direction == StaticTileController.Direction.down)
        {
            tilesToRemoveOccupy = new List<VirtualGrid>(tilesAtSameDirection);
            
            for (int i = 0; i < tilesAtSameDirection.Count; i++)
            {
                for (int j = 0; j < listOfTilesBetweenGridValues.Count; j++)
                {
                    if (tilesAtSameDirection[i].z == listOfTilesBetweenGridValues[j] && tilesToRemoveOccupy.Contains(tilesAtSameDirection[i]))
                    {
                        tilesToRemoveOccupy.Remove(tilesAtSameDirection[i]);
                    }
                }
            }
        }
        else if (direction == StaticTileController.Direction.right)
        {
            tilesToRemoveOccupy = new List<VirtualGrid>(tilesAtSameDirection);
            
            for (int i = 0; i < tilesAtSameDirection.Count; i++)
            {
                for (int j = 0; j < listOfTilesBetweenGridValues.Count; j++)
                {
                    if (tilesAtSameDirection[i].x == listOfTilesBetweenGridValues[j] && tilesToRemoveOccupy.Contains(tilesAtSameDirection[i]))
                    {
                        tilesToRemoveOccupy.Remove(tilesAtSameDirection[i]);
                    }
                }
            }
        }
        else if (direction == StaticTileController.Direction.left)
        {
            tilesToRemoveOccupy = new List<VirtualGrid>(tilesAtSameDirection);
            
            for (int i = 0; i < tilesAtSameDirection.Count; i++)
            {
                for (int j = 0; j < listOfTilesBetweenGridValues.Count; j++)
                {
                    if (tilesAtSameDirection[i].x == listOfTilesBetweenGridValues[j] && tilesToRemoveOccupy.Contains(tilesAtSameDirection[i]))
                    {
                        tilesToRemoveOccupy.Remove(tilesAtSameDirection[i]);
                    }
                }
            }
        }

        return tilesToRemoveOccupy;
    }
    
    private List<int> FindGridValuesOfTilesBetweenStaticTile()
    {
        int x = virtualGrid.x;
        int z = virtualGrid.z;

        int sx = staticTile.virtualGrid.x;
        int sz = staticTile.virtualGrid.z;

        List<int> listOfTilesBetweenGridValues = new List<int>();

        if (direction == StaticTileController.Direction.top)
        {
            for (int i = sz + 1; i < z + 1; i++)
            {
                listOfTilesBetweenGridValues.Add(i);
            }
        }
        else if (direction == StaticTileController.Direction.down)
        {
            for (int i = sz - 1; i > z - 1; i--)
            {
                listOfTilesBetweenGridValues.Add(i);
            }
        }
        else if (direction == StaticTileController.Direction.right)
        {
            for (int i = sx + 1; i < x + 1; i++)
            {
                listOfTilesBetweenGridValues.Add(i);
            }
        }
        else if (direction == StaticTileController.Direction.left)
        {
            for (int i = sx - 1; i > x - 1; i--)
            {
                listOfTilesBetweenGridValues.Add(i);
            }
        }
        


        return listOfTilesBetweenGridValues;
    }
}
