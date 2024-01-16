using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailManager : SingletonMonoBehaviour<FailManager>
{
    public bool isLevelFailed;
    public bool isLevelCompleted;
    
    void Start()
    {
        isLevelFailed = false;
        isLevelCompleted = false;
    }
    
    protected override void Awake()
    {
        base.Awake();
    }
}