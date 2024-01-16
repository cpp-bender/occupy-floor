using UnityEngine;

[CreateAssetMenu(menuName = "Occupy Floor/Enviroment Data", fileName = "Enviroment Data")]
public class EnviromentData : ScriptableObject
{
    public GameObject framePrefab;
    public GameObject doorsPrefab;
    public GameObject groundCirclePrefab;
    public Vector3 discoBallPos;

    public EnviromentSettings enviromentSettings;

    public void SetFrame()
    {
        var frame = Instantiate(framePrefab);
        frame.transform.position = enviromentSettings.framePos;
        frame.transform.localScale = Vector3.one * enviromentSettings.frameScale;
    }

    public void SetCircleGround()
    {
        var circleGround = Instantiate(groundCirclePrefab);

        var groundManager = FindObjectOfType<GroundManager>();

        circleGround.transform.SetParent(groundManager.transform);

        circleGround.transform.position = enviromentSettings.circlePos;
        circleGround.transform.localScale = Vector3.one * enviromentSettings.circleScale;

        var circleGroundTextObj = circleGround.transform.GetChild(0).gameObject;

        groundManager.groundText.Insert(0, circleGroundTextObj);

        groundManager.SetGroundsText();
    }

    public void SetDoors()
    {
        var doors = Instantiate(doorsPrefab);

        doors.transform.position = enviromentSettings.doorsPos;
    }
}
