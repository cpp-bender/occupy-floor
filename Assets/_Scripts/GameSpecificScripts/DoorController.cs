using UnityEngine;
using DG.Tweening;

public class DoorController : MonoBehaviour
{
    public int sumOfTiles;
    //To be changed
    public int pressedTiles;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float doorDistance;

    private float doorStartPos;
    private float distance;
    private float neededPercantageValue;
    private LevelManager levelManager;

    void Start()
    {
        doorStartPos = transform.localPosition.x;
        levelManager = FindObjectOfType<LevelManager>();
        //HandleDoor(pressedTiles);
    }

    public void HandleDoor()
    {
        sumOfTiles = levelManager.walkableTilesParent.transform.childCount;
        pressedTiles = 0;
        
        for (int i = 0; i < levelManager.walkableTilesParent.transform.childCount; i++)
        {
            if (levelManager.walkableTilesParent.transform.GetChild(i).GetComponent<TileController>().isOccupied)
            {
                pressedTiles++;
            }
        }
        
        
        neededPercantageValue = (float)pressedTiles / (float)sumOfTiles;
        distance = doorDistance * neededPercantageValue;
        GetTouchMoveTween();
    }

    private void GetTouchMoveTween()
    {
        leftDoor.transform.DOMoveX(doorStartPos - distance, 0.5f)
         .SetEase(Ease.Linear)
         .Play();

        rightDoor.transform.DOMoveX(doorStartPos + distance, 0.5f)
         .SetEase(Ease.Linear)
         .Play();
    }
}
