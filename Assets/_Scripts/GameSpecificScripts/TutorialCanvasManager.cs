using DG.Tweening;
using UnityEngine;

public class TutorialCanvasManager : MonoBehaviour
{
    [Header("DEPENDENCIES")]
    public GameObject hand;

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DOTween.Kill("handtutorialtween");
        }
    }
}
