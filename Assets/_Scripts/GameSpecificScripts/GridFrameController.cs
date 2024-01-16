using UnityEngine;

public class GridFrameController : SingletonMonoBehaviour<GridFrameController>
{
    public Material levelEndMat;

    private MeshRenderer meshRenderer;

    protected override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColor()
    {
        var materials = meshRenderer.materials;
        materials[1] = levelEndMat;
        meshRenderer.materials = materials;
    }
}
