using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var levelManager = (LevelManager)target;

        if (GUILayout.Button("Spawn Level End"))
        {
            levelManager.SpawnLevelEnd();
        }
    }
}
