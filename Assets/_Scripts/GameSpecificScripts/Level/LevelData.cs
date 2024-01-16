using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Occupy Floor/Level Data", fileName = "Level Data")]
public class LevelData : ScriptableObject
{
    [Header("GRID SETTINGS"), Space(5f)]
    public Vector3 initialPos = Vector3.zero;
    public int width = 5;
    public int height = 5;
    public float widthThreshold = 2f;
    public float heightThreshold = 2f;

    [Header("TILE OBJECTS")]
    public List<StaticTileData> staticTiles;
    public List<WalkableTileData> walkableTiles;
    public List<ObstacleTileData> obstacleTiles;

    [Header("ENVIROMENT")]
    public EnviromentData enviromentData;

    [Header("CAMERA")]
    public CameraData cameraData;

    public bool canPlayTutorial = false;
}
