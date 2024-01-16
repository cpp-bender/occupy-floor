using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TileController : BaseTileController
{
    public enum TileType { Static, Walkable, Obstacle }
    public TileType tileType;
    
    public VirtualGrid virtualGrid;
    
    public GameObject stickManPrefab;
    public GameObject button;
    
    public List<VirtualGrid> tilesAll;
    public List<VirtualGrid> tilesTop;
    public List<VirtualGrid> tilesDown;
    public List<VirtualGrid> tilesRight;
    public List<VirtualGrid> tilesLeft;
    
    public enum MoveDirection { Undecided, Right, Left, Top, Down }

    public MoveDirection startDirection;
    public MoveDirection relativeDirection;

    public TileController parentStaticTile;
    public TileController parentWalkableTile;
    
    public int number;
    public TextMeshProUGUI numberText;
    
    public List<GameObject> stickMen;
    public List<Transform> stickManSpawnPoints;
    public List<GameObject> stickMenAtStatic;
    public List<GameObject> stickMenAtWalkable;
    public List<GameObject> stickMenLastMove;
    private int lastMoveCount;
    private int previousLastMoveCount;
    public StickMan stickManOnTile;
    public List<VirtualGrid> listOfTile;

    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Color originalColor;
    public bool isOccupied;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        virtualGrid = GridManager.Instance.WorldToGrid(transform.position);
        
        if (tileType != TileType.Obstacle)
        {
            originalColor = skinnedMeshRenderer.material.color;
        }
        
        FindDirectionalTiles();
    }

    private void FindDirectionalTiles()
    {
        if (tileType != TileType.Static)
            return;
        
        int x = virtualGrid.x;
        int z = virtualGrid.z;

        for (int i = 0; i < GridManager.Instance.virtualGrid.Count; i++)
        {
            VirtualGrid tile = GridManager.Instance.virtualGrid[i];

            
            int dx = tile.x;
            int dz = tile.z;
            
            if (x == dx) // top-down
            {
                if (z < dz) // top
                {
                    tilesTop.Add(tile);
                }
                else if (z > dz) // down
                {
                    tilesDown.Add(tile);
                }
            }
            else if (z == dz) // right-left
            {
                if (x < dx) // right
                {
                    tilesRight.Add(tile);
                }
                else if (x > dx) // left
                {
                    tilesLeft.Add(tile);
                }
            }
        }
        
        List<VirtualGrid> sortedTop = tilesTop.OrderBy(grid => grid.z).ToList();
        List<VirtualGrid> sortedDown = tilesDown.OrderByDescending(grid => grid.z).ToList();
        List<VirtualGrid> sortedRight = tilesRight.OrderBy(grid => grid.x).ToList();
        List<VirtualGrid> sortedLeft = tilesLeft.OrderByDescending(grid => grid.x).ToList();

        tilesTop = CleanSort(sortedTop);
        tilesDown = CleanSort(sortedDown);
        tilesRight = CleanSort(sortedRight);
        tilesLeft = CleanSort(sortedLeft);
    }

    private List<VirtualGrid> CleanSort(List<VirtualGrid> sorted)
    {
        List<VirtualGrid> cleanSort = new List<VirtualGrid>();
        
        for (int i = 0; i < sorted.Count; i++)
        {
            TileController tileController = sorted[i].tileObj.GetComponent<TileController>();

            if (tileController.tileType != TileType.Walkable)
            {
                break;
            }
            else
            {
                cleanSort.Add(sorted[i]);
            }
        }

        return cleanSort;
    }

    public void Occupy(VirtualGrid previousTile, VirtualGrid currentTile)
    {
        if (tilesTop.Contains(currentTile))
        {
            // Debug.LogError("Top");

            List<VirtualGrid> tilesToMove = FindTilesToMove(tilesTop, currentTile);
            List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesTop, tilesToMove);
            Move(tilesToMove);
            Return(tilesToReturn);
        }
        else if (tilesDown.Contains(currentTile))
        {
            // Debug.LogError("Down");
            
            List<VirtualGrid> tilesToMove = FindTilesToMove(tilesDown, currentTile);
            List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesDown, tilesToMove);
            Move(tilesToMove);
            Return(tilesToReturn);
        }
        else if (tilesRight.Contains(currentTile))
        {
            // Debug.LogError("Right");
            
            List<VirtualGrid> tilesToMove = FindTilesToMove(tilesRight, currentTile);
            List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesRight, tilesToMove);
            Move(tilesToMove);
            Return(tilesToReturn);
        }
        else if (tilesLeft.Contains(currentTile))
        {
            // Debug.LogError("Left");
            
            List<VirtualGrid> tilesToMove = FindTilesToMove(tilesLeft, currentTile);
            List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesLeft, tilesToMove);
            Move(tilesToMove);
            Return(tilesToReturn);
        }
        else if (virtualGrid == currentTile)
        {
            if (tilesTop.Contains(previousTile))
            {
                // Debug.LogError("Top");

                List<VirtualGrid> tilesToMove = new List<VirtualGrid>();
                List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesTop, tilesToMove);
                Move(tilesToMove);
                Return(tilesToReturn);
            }
            else if (tilesDown.Contains(previousTile))
            {
                // Debug.LogError("Down");
            
                List<VirtualGrid> tilesToMove = new List<VirtualGrid>();
                List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesDown, tilesToMove);
                Move(tilesToMove);
                Return(tilesToReturn);
            }
            else if (tilesRight.Contains(previousTile))
            {
                // Debug.LogError("Right");
            
                List<VirtualGrid> tilesToMove = new List<VirtualGrid>();
                List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesRight, tilesToMove);
                Move(tilesToMove);
                Return(tilesToReturn);
            }
            else if (tilesLeft.Contains(previousTile))
            {
                // Debug.LogError("Left");

                List<VirtualGrid> tilesToMove = new List<VirtualGrid>();
                List<VirtualGrid> tilesToReturn = FindTilesToReturn(tilesLeft, tilesToMove);
                Move(tilesToMove);
                Return(tilesToReturn);
            }

        }
        
        stickMen[stickMen.Count - 1].GetComponent<StickMan>().SetCount(stickMenAtStatic.Count - 1);
        FindObjectOfType<DoorController>().HandleDoor();
    }

    private List<VirtualGrid> FindTilesToMove(List<VirtualGrid> tiles, VirtualGrid v)
    {
        List<VirtualGrid> tilesToMove = new List<VirtualGrid>();

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == v)
            {
                tilesToMove.Add(tiles[i]);
                break;
            }
            else
            {
                tilesToMove.Add(tiles[i]);
            }
        }

        return tilesToMove;
    }
    
    private List<VirtualGrid> FindTilesToReturn(List<VirtualGrid> tiles, List<VirtualGrid> tilesToMove)
    {
        List<VirtualGrid> tilesTemp = new List<VirtualGrid>(tiles);

        for (int i = 0; i < tilesToMove.Count; i++)
        {
            tilesTemp.Remove(tilesToMove[i]);
        }

        List<VirtualGrid> tilesToReturn = new List<VirtualGrid>(tilesTemp);
        
        // Debug.LogError(tilesToReturn.Count);
        
        return tilesToReturn;
    }

    private void Move(List<VirtualGrid> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tileController = tiles[i].tileObj.GetComponent<TileController>();

            if (tileController.stickManOnTile == null)
            {
                if (stickMenAtStatic.Count > 1)
                {
                    StickMan stickMan = stickMen[stickMenAtWalkable.Count].GetComponent<StickMan>();
                    stickMan.transform.DOMove(tiles[i].tileObj.transform.position + Vector3.up * 0.2f, 0.1f).Play().OnComplete(
                        delegate
                        {
                            tileController.Press();


                        });
                
                    stickMenAtStatic.Remove(stickMan.gameObject);
                    if (!stickMenAtWalkable.Contains(stickMan.gameObject))
                    {
                        stickMenAtWalkable.Add(stickMan.gameObject);
                    }
                    tileController.stickManOnTile = stickMan;
                    tileController.parentStaticTile = this;
                }
            }
        }
    }

    private void Return(List<VirtualGrid> tiles)
    {
        // Debug.LogError(tiles.Count);
        
        for (int i = 0; i < tiles.Count; i++)
        {
            TileController tileController = tiles[i].tileObj.GetComponent<TileController>();

            if (tileController.parentStaticTile == this)
            {
                StickMan stickMan = tileController.stickManOnTile;

                if (stickMan != null)
                {
                    // Debug.LogError("Here");
                
                    stickMan.transform.DOMove(stickManSpawnPoints[stickMan.originalIndex].position, 0.1f).Play().OnComplete(
                        delegate
                        {

                            tileController.UnPress();
                        });
                
                    stickMenAtWalkable.Remove(stickMan.gameObject);
                    if (!stickMenAtStatic.Contains(stickMan.gameObject))
                    {
                        stickMenAtStatic.Add(stickMan.gameObject);
                    }
                    tileController.stickManOnTile = null;
                }
            }


            

        }
    }

    private int FindRelativeIndex(VirtualGrid v)
    {
        int position = 0;
        
        if (tilesTop.Contains(v))
        {
            position = tilesTop.IndexOf(v);
        }
        else if (tilesDown.Contains(v))
        {
            position = tilesDown.IndexOf(v);
        }
        else if (tilesRight.Contains(v))
        {
            position = tilesRight.IndexOf(v);
        }
        else if (tilesLeft.Contains(v))
        {
            position = tilesLeft.IndexOf(v);
        }

        Debug.LogError(position);
        return position;
    }


    public void Press()
    {
        skinnedMeshRenderer.material.color = stickManOnTile.skinnedMeshRenderer.material.color;
        isOccupied = true;
    }

    public void UnPress()
    {
        skinnedMeshRenderer.material.color = originalColor;
        isOccupied = false;
    }
    
    public void UnPressSoft()
    {
        skinnedMeshRenderer.material.color = originalColor;
        isOccupied = false;
    }
    
    private MoveDirection DetectDirection(VirtualGrid current)
    {
        MoveDirection moveDirection = MoveDirection.Top;
        
        int x = virtualGrid.x;
        int z = virtualGrid.z;

        int dx = current.x;
        int dz = current.z;
        
        if (x == dx) // top-down
        {
            if (z < dz) // top
            {
                moveDirection = MoveDirection.Top;
            }
            else if (z > dz) // down
            {
                moveDirection = MoveDirection.Down;
            }
        }
        else if (z == dz) // right-left
        {
            if (x < dx) // right
            {
                moveDirection = MoveDirection.Right;
            }
            else if (x > dx) // left
            {
                moveDirection = MoveDirection.Left;
            }
        }

        return moveDirection;
    }
    
    private void OccupyTile(VirtualGrid vG)
    {
        // Debug.LogError("Occupy");
        
        MoveDirection moveDirection = DetectDirection(vG);
        
        if (!tilesAll.Contains(vG))
        {
            tilesAll.Add(vG);
            // vG.tileObj.GetComponent<TileController>().Press();
            
            if (moveDirection == MoveDirection.Top) // top
            {
                tilesTop.Add(vG);
                vG.tileObj.GetComponent<TileController>().listOfTile = tilesTop;
                listOfTile = tilesTop;
            }
            else if (moveDirection == MoveDirection.Down) // down
            {
                tilesDown.Add(vG);
                vG.tileObj.GetComponent<TileController>().listOfTile = tilesDown;
                listOfTile = tilesDown;
            }
            else if (moveDirection == MoveDirection.Right) // right
            {
                tilesRight.Add(vG);
                vG.tileObj.GetComponent<TileController>().listOfTile = tilesRight;
                listOfTile = tilesRight;
            }
            else if (moveDirection == MoveDirection.Left) // left
            {
                tilesLeft.Add(vG);
                vG.tileObj.GetComponent<TileController>().listOfTile = tilesLeft;
                listOfTile = tilesLeft;
            }
        }
    }
    
    private void RemoveTile(VirtualGrid vG)
    {
        if (tilesAll.Contains(vG))
        {
            tilesAll.Remove(vG);
            vG.tileObj.GetComponent<TileController>().UnPress();
            
            TileController tileController = vG.tileObj.GetComponent<TileController>();
            
            if (tileController.relativeDirection == MoveDirection.Top) // top
            {
                
                listOfTile = new List<VirtualGrid>(tilesTop);
                // tilesTop.Remove(vG);
                // vG.tileObj.GetComponent<TileController>().listOfTile = tilesTop;
                
            }
            else if (tileController.relativeDirection == MoveDirection.Down) // down
            {
                // Debug.LogError("Hereeeeee");
                listOfTile = new List<VirtualGrid>(tilesDown);
                // tilesDown.Remove(vG);
                // vG.tileObj.GetComponent<TileController>().listOfTile = tilesDown;
            }
            else if (tileController.relativeDirection == MoveDirection.Right) // right
            {
                listOfTile = new List<VirtualGrid>(tilesRight);
                // tilesRight.Remove(vG);
                // vG.tileObj.GetComponent<TileController>().listOfTile = tilesRight;
            }
            else if (tileController.relativeDirection == MoveDirection.Left) // left
            {
                listOfTile = new List<VirtualGrid>(tilesLeft);
                // tilesLeft.Remove(vG);
                // vG.tileObj.GetComponent<TileController>().listOfTile = tilesLeft;
            }
        }
    }

    public void StartOccupyTile(VirtualGrid current)
    {
        // Debug.LogError("Start");
        
        startDirection = DetectDirection(current);
        
        TileController tileController = current.tileObj.GetComponent<TileController>();
        
        if (tileController.isOccupied && !parentStaticTile.tilesAll.Contains(current))
            return;
        
        // Debug.LogError("3");
        
        OccupyTile(current);
        
        SetDirection(startDirection);
        
        tileController.parentStaticTile = tileRaycastManager.startStaticTile;
        tileController.relativeDirection = startDirection;;
        
        tileController.parentStaticTile.SetNumbers(tileController.relativeDirection, true);
        tileController.parentStaticTile.DistributeStickMen();
        tileController.parentStaticTile.ClearMovements();

    }
    
    public void ContinueOccupyTile(VirtualGrid current)
    {
        if (parentStaticTile == null)
            return;
        
        // Debug.LogError("3");
        
        TileController tileController = current.tileObj.GetComponent<TileController>();
        
        if (tileController.isOccupied && !parentStaticTile.tilesAll.Contains(current))
            return;
        
        // Debug.LogError("4");
        
        MoveDirection moveDirection = DetectDirection(current);
        current.tileObj.GetComponent<TileController>().parentStaticTile = tileRaycastManager.startStaticTile;
        
        
        if (tileController.tileType == TileType.Walkable)
        {
            tileController.relativeDirection = parentStaticTile.startDirection;
        }
        


        if (parentStaticTile != null)
        {
            if (parentStaticTile.startDirection == MoveDirection.Top)
            {
                if (moveDirection == MoveDirection.Top)
                {
                    if (parentStaticTile.stickMenAtStatic.Count <= 1 || current.tileObj.GetComponent<TileController>().stickManOnTile != null || current.tileObj.GetComponent<TileController>().tileType == TileType.Obstacle) 
                        return;
                    
                    parentStaticTile.OccupyTile(current);
                    parentStaticTile.SetNumbers(tileController.relativeDirection, true);
                    parentStaticTile.DistributeStickMen();
                    parentStaticTile.ClearMovements();
                }
                else if (moveDirection == MoveDirection.Down)
                {
                    parentStaticTile.SetNumbers(relativeDirection, false);
                    parentStaticTile.RemoveTile(virtualGrid);
                    parentStaticTile.DistributeStickMen();
                    
                    parentStaticTile.ReturnHome();
                    parentStaticTile.ClearMovements();
                }
                

            }
            else if (parentStaticTile.startDirection == MoveDirection.Down)
            {
                
                if (moveDirection == MoveDirection.Down)
                {
                    if (parentStaticTile.stickMenAtStatic.Count <= 1 || current.tileObj.GetComponent<TileController>().stickManOnTile != null || current.tileObj.GetComponent<TileController>().tileType == TileType.Obstacle) 
                        return;
                    
                    parentStaticTile.OccupyTile(current);
                    parentStaticTile.SetNumbers(tileController.relativeDirection, true);
                    parentStaticTile.DistributeStickMen();
                    parentStaticTile.ClearMovements();
                    
                }
                else if (moveDirection == MoveDirection.Top)
                {
                    // Debug.LogError("5");
                    
                    parentStaticTile.SetNumbers(relativeDirection, false);
                    parentStaticTile.RemoveTile(virtualGrid);
                    parentStaticTile.DistributeStickMen();
                                        
                    parentStaticTile.ReturnHome();
                    parentStaticTile.ClearMovements();
                }
                
                
            }
            else if (parentStaticTile.startDirection == MoveDirection.Right)
            {
                if (moveDirection == MoveDirection.Right)
                {
                    if (parentStaticTile.stickMenAtStatic.Count <= 1 || current.tileObj.GetComponent<TileController>().stickManOnTile != null || current.tileObj.GetComponent<TileController>().tileType == TileType.Obstacle) 
                        return;
                    
                    parentStaticTile.OccupyTile(current);
                    parentStaticTile.SetNumbers(tileController.relativeDirection, true);
                    parentStaticTile.DistributeStickMen();
                    parentStaticTile.ClearMovements();
                }
                else if (moveDirection == MoveDirection.Left)
                {
                    parentStaticTile.SetNumbers(relativeDirection, false);
                    parentStaticTile.RemoveTile(virtualGrid);
                    parentStaticTile.DistributeStickMen();
                                        
                    parentStaticTile.ReturnHome();
                    parentStaticTile.ClearMovements();
                }
                
                
            }
            else if (parentStaticTile.startDirection == MoveDirection.Left)
            {
                if (moveDirection == MoveDirection.Left)
                {
                    if (parentStaticTile.stickMenAtStatic.Count <= 1 || current.tileObj.GetComponent<TileController>().stickManOnTile != null || current.tileObj.GetComponent<TileController>().tileType == TileType.Obstacle) 
                        return;
                    
                    parentStaticTile.OccupyTile(current);
                    parentStaticTile.SetNumbers(tileController.relativeDirection, true);
                    parentStaticTile.DistributeStickMen();
                    parentStaticTile.ClearMovements();
                }
                else if (moveDirection == MoveDirection.Right)
                {
                    parentStaticTile.SetNumbers(relativeDirection, false);
                    parentStaticTile.RemoveTile(virtualGrid);
                    parentStaticTile.DistributeStickMen();
                                        
                    parentStaticTile.ReturnHome();
                    parentStaticTile.ClearMovements();
                }
            }
        }
    }

    private void SetNumbers(MoveDirection moveDirection, bool increase)
    {
        if (moveDirection == MoveDirection.Top) // top
        {
            for (int i = 0; i < tilesTop.Count; i++)
            {
                if (increase)
                {
                    tilesTop[i].tileObj.GetComponent<TileController>().IncreaseNumber();
                }
                else
                {
                    tilesTop[i].tileObj.GetComponent<TileController>().DecreaseNumber();
                }
            }
        }
        else if (moveDirection == MoveDirection.Down) // down
        {
            for (int i = 0; i < tilesDown.Count; i++)
            {
                if (increase)
                {
                    tilesDown[i].tileObj.GetComponent<TileController>().IncreaseNumber();
                }
                else
                {
                    tilesDown[i].tileObj.GetComponent<TileController>().DecreaseNumber();                
                }
            }
        }
        else if (moveDirection == MoveDirection.Right) // right
        {
            for (int i = 0; i < tilesRight.Count; i++)
            {
                if (increase)
                {
                    tilesRight[i].tileObj.GetComponent<TileController>().IncreaseNumber();
                }
                else
                {
                    tilesRight[i].tileObj.GetComponent<TileController>().DecreaseNumber();
                }
            }
        }
        else if (moveDirection == MoveDirection.Left) // left
        {
            for (int i = 0; i < tilesLeft.Count; i++)
            {
                if (increase)
                {
                    tilesLeft[i].tileObj.GetComponent<TileController>().IncreaseNumber();
                }
                else
                {
                    tilesLeft[i].tileObj.GetComponent<TileController>().DecreaseNumber();
                }
            }
        }

        
    }

    public void Place()
    {
        if (parentStaticTile != null)
        {
            // Debug.LogError("Here");
            parentStaticTile.startDirection = relativeDirection;
        
            for (int i = 0; i < listOfTile.Count; i++)
            {
                listOfTile[i].tileObj.GetComponent<TileController>().stickManOnTile.isPlaced = false;
            }
        }

    }
    
    private void SetNumber(int num)
    {
        number = num;
    }
    
    private void IncreaseNumber()
    {
        number++;
        numberText.text = number.ToString();
    }
    
    private void DecreaseNumber()
    {
        number--;

        if (number <= 0)
        {
            number = 0;
            numberText.text = " ";
        }
        else
        {
            numberText.text = number.ToString();
        }
    }
    
    public void DistributeStickMen()
    {
        // Debug.LogError("6");
        
        for (int i = 0; i < tilesAll.Count; i++)
        {
            for (int j = 0; j < stickMen.Count - 1; j++)
            {
                StickMan stickManController = stickMen[j].GetComponent<StickMan>();

                
                
                if (!stickManController.isPlaced && stickManController.number == tilesAll[i].tileObj.GetComponent<TileController>().number && stickManController.direction == tilesAll[i].tileObj.GetComponent<TileController>().relativeDirection)
                {
                    
                    stickManController.direction = tilesAll[i].tileObj.GetComponent<TileController>().relativeDirection;
                    // Debug.LogError("Dist");
                    var i1 = i;
                    stickManController.currentTile = tilesAll[i];
                    stickMen[j].transform.DOMove(tilesAll[i].tileObj.transform.position + Vector3.up * 0.2f, 0.1f).Play().OnComplete(
                        delegate
                        {
                            stickManController.isOnStatic = false;
                            tilesAll[i1].tileObj.GetComponent<TileController>().stickManOnTile = stickManController;
                            tilesAll[i1].tileObj.GetComponent<TileController>().Press();
                        });
                    
                    // tilesAll[i].tileObj.GetComponent<TileController>().UnPress();
                    
                    
                    
                    if (!stickMenAtWalkable.Contains(stickMen[j]))
                    {
                        stickMenAtWalkable.Add(stickMen[j]);
                    }
                    
                    stickMenAtStatic.Remove(stickMen[j]);
                }


            }
        }
        
        // Return


    }

    private void ClearMovements()
    {
        // Debug.LogError("Here");
        stickMen[stickMen.Count - 1].GetComponent<StickMan>().SetCount(stickMenAtStatic.Count - 1);
        FindObjectOfType<DoorController>().HandleDoor();
    }

    private void ReturnHome()
    {
        // Debug.LogError("Return");
        
        // Debug.LogError("7");

        int originalIndexOfStickManOfCurrentDirectionLastTile = 0;
        
        if (listOfTile.Count > 0 && listOfTile[0].tileObj.GetComponent<TileController>().stickManOnTile != null)
        {
            originalIndexOfStickManOfCurrentDirectionLastTile = listOfTile[0].tileObj
                .GetComponent<TileController>().stickManOnTile.originalIndex;
            // Debug.LogError("8 " + originalIndexOfStickManOfCurrentDirectionLastTile);
            // Debug.LogError(listOfTile[0].x + " " + listOfTile[0].z);
        }
        else
        {
            originalIndexOfStickManOfCurrentDirectionLastTile = tilesAll.Count;

        }

        // Debug.LogError(originalIndexOfStickManOfCurrentDirectionLastTile);
        
        if (originalIndexOfStickManOfCurrentDirectionLastTile >= stickMen.Count)
            return;


        StickMan stickManController = stickMen[originalIndexOfStickManOfCurrentDirectionLastTile].GetComponent<StickMan>();

        if (!stickManController.isOnStatic)
        {
            // Debug.LogError("Return");
            // Debug.LogError("Return " + originalIndexOfStickManOfCurrentDirectionLastTile);
            stickManController.currentTile = virtualGrid;
            stickMen[originalIndexOfStickManOfCurrentDirectionLastTile].transform.DOMove(stickManSpawnPoints[originalIndexOfStickManOfCurrentDirectionLastTile].position, 0.1f).Play();
            stickManController.isOnStatic = true;
            
            // listOfTile[0].tileObj.GetComponent<TileController>().UnPress();
            
            stickMenAtWalkable.Remove(stickMen[originalIndexOfStickManOfCurrentDirectionLastTile]);
            // stickManController.currentTile.tileObj.GetComponent<TileController>().UnPress();
                
            if (!stickMenAtStatic.Contains(stickMen[originalIndexOfStickManOfCurrentDirectionLastTile]))
            {
                stickMenAtStatic.Add(stickMen[originalIndexOfStickManOfCurrentDirectionLastTile]);
            }
        }
        

    }

    public void SaveTiles()
    {
        for (int i = 0; i < stickMenAtWalkable.Count; i++)
        {
            stickMenAtWalkable[i].GetComponent<StickMan>().isPlaced = true;
        }

        for (int i = 0; i < stickMenAtStatic.Count; i++)
        {
            stickMenAtStatic[i].GetComponent<StickMan>().isPlaced = false;
        }
    }

    public void LastMove()
    {
        stickMenLastMove = new List<GameObject>(stickMenAtWalkable);
        lastMoveCount = stickMenLastMove.Count - previousLastMoveCount;
        previousLastMoveCount = stickMenLastMove.Count;
    }

    public void UpdateNumbers()
    {
        // Debug.LogError("Here2");
        
        LastMove();
        
        for (int i = 0; i < stickMenAtStatic.Count; i++)
        {
            // stickMenAtStatic[i].GetComponent<StickMan>().number -= stickMenAtWalkable.Count;
            // stickMenAtStatic[i].GetComponent<StickMan>().number -= lastMoveCount;
            stickMenAtStatic[i].GetComponent<StickMan>().number =
                stickMenAtStatic[i].GetComponent<StickMan>().originalIndex + 1 - stickMenAtWalkable.Count;
            // Debug.LogError(lastMoveCount);
            stickMenAtStatic[i].GetComponent<StickMan>().UpdateShownNumber();
            
        }
    }

    private void SetDirection(MoveDirection moveDirection)
    {
        for (int i = 0; i < stickMenAtStatic.Count; i++)
        {
            stickMenAtStatic[i].GetComponent<StickMan>().direction = moveDirection;
        }
    }
    
    public void UpdateNumbersAtHome()
    {
        // Debug.LogError("Here");
        
        // LastMove();

        int counter = 0;

        for (int j = 0; j < stickMen.Count; j++)
        {
            for (int i = 0; i < stickMenAtStatic.Count; i++)
            {
                if (stickMen[j].GetComponent<StickMan>() == stickMenAtStatic[i].GetComponent<StickMan>())
                {
                    counter++;

                    stickMenAtStatic[i].GetComponent<StickMan>().number = counter;
                    stickMenAtStatic[i].GetComponent<StickMan>().UpdateShownNumber();
                }
                
                // stickMenAtStatic[i].GetComponent<StickMan>().number = stickMenAtStatic[i].GetComponent<StickMan>().originalIndex + 1 - stickMenAtWalkable.Count;
                // Debug.LogError((stickMenAtStatic[i].GetComponent<StickMan>().originalIndex + 1) + " " + stickMenAtWalkable.Count);
                // stickMenAtStatic[i].GetComponent<StickMan>().UpdateShownNumber();
            }
        }


    }
    
    public void SetColor(Color color)
    {
        button.GetComponent<SkinnedMeshRenderer>().material.color = color;
    }
    
}
