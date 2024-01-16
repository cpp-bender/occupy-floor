using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Occupy Floor/Camera Settings", fileName = "Camera Settings")]
public class CameraData : ScriptableObject
{
    public CameraSettings cameraSettings;

    public void SetCam()
    {
        var mainCam = Camera.main;

        mainCam.transform.position = cameraSettings.worldPos;
        mainCam.transform.rotation = Quaternion.Euler(cameraSettings.rotation);
    }
}
