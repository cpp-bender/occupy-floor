using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StickMan : MonoBehaviour
{
    public Transform numberParent;
    public int originalIndex;
    public int number;
    public TileController.MoveDirection direction;
    public bool isOnStatic;
    public bool isPlaced;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public int count;
    public VirtualGrid currentTile;
    
    void Start()
    {
        isOnStatic = true;

        numberParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().outlineColor =
            skinnedMeshRenderer.material.color;
        
        UpdateShownNumber();
    }
    
    void Update()
    {
        numberParent.transform.LookAt(Camera.main.transform);
    }

    public void SetNumber(int num)
    {
        number = num - 1;
        originalIndex = num - 1;
    }

    public void UpdateShownNumber()
    {
        numberParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = number.ToString();
    }
    
    public void SetCount(int num)
    {
        count = num;

        UpdateRemainingStickManCount();
    }
    
    public void UpdateRemainingStickManCount()
    {
        string text = count.ToString();
        
        // if (count == 0)
        // {
        //     text = "";
        // }

        numberParent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
    }
}
