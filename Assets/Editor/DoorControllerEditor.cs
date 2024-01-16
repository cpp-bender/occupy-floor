using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var doorController = (DoorController)target;

        if (GUILayout.Button("Handle Door"))
        {
            doorController.HandleDoor();
        }
    }
}
