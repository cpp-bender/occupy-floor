using UnityEngine;

public class BaseTileController : MonoBehaviour
{
    protected RaycastManager raycastManager;
    protected TileRaycastManager tileRaycastManager;

    protected virtual void Awake()
    {
        raycastManager = FindObjectOfType<RaycastManager>();
        tileRaycastManager = FindObjectOfType<TileRaycastManager>();
    }

    protected virtual void Start()
    {

    }
}
