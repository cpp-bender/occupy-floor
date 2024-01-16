using DG.Tweening;
using UnityEngine;
using System;

[Serializable]
public class DiscoBallData
{
    public GameObject discoBall;
    public BaseTweenData moveTweenData;

    private Tween moveTween;

    public void DOMoveDiscoBall()
    {
        var enviromentData = LevelManager.Instance.levels[LevelManager.Instance.levelIndex].enviromentData;

        discoBall.transform.localPosition = enviromentData.discoBallPos;

        moveTween = moveTweenData.GetTween(discoBall)
            .Play();
    }
}
