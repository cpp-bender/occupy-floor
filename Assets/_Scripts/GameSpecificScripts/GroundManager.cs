using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundManager : MonoBehaviour
{
    [Header("DEPENDENCIES")]
    public List<GameObject> groundText;

    public void SetGroundsText()
    {
        var currentLevelCount = GameManager.instance.currentLevel;
        var tempLevelCount = currentLevelCount;

        foreach (var text in groundText)
        {
            text.transform.GetComponent<TextMeshPro>().text = tempLevelCount.ToString();
            tempLevelCount++;
        }
    }
}
