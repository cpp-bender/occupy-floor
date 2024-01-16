using UnityEngine;
using System;

[Serializable]
public class VirtualGrid
{
    public int x;
    public int z;
    public GameObject tileObj;

    public VirtualGrid(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void SetGameObject(GameObject go)
    {
        tileObj = go;
    }
}
