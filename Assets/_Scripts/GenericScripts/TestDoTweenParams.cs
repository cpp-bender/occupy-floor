using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestDoTweenParams : MonoBehaviour
{
    public TweenParamsData tweenParams;

    void Start()
    {
        transform.DOMoveY(2f, 1f).SetAs(tweenParams.TweenParams).Play();
    }
}
