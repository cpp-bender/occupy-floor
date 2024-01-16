using UnityEngine;
using System;

[Serializable]
public class EnviromentSettings
{
    [Header("FRAME")]
    public Vector3 framePos;
    public float frameScale;

    [Header("CIRCLE GROUND")]
    public Vector3 circlePos;
    public float circleScale;

    [Header("DOORS")]
    public Vector3 doorsPos;
}
