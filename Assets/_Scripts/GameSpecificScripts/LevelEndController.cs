using UnityEngine;

public class LevelEndController : MonoBehaviour
{
    [Header("DEPENDENCIES")]
    public DiscoBallData discoBallData;

    private void Start()
    {
        discoBallData.DOMoveDiscoBall();
    }
}
