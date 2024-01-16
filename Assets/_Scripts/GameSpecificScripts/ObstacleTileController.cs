using UnityEngine;

[SelectionBase]
public class ObstacleTileController : BaseTileController
{
    [Header("DEPENDENCIES")]
    public GameObject speaker;
    public GameObject floor;
    public GameObject speakerParticle;

    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        HideSpeaker();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void HandleForLevelEnd()
    {
        ShowSpeaker();

        SpawnSpeakerParticle();

        PlaySpeakerAnim();

        PlayFloorAnim();
    }

    private void HideSpeaker()
    {
        speaker.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 100);
    }

    private void ShowSpeaker()
    {
        speaker.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, 0);
    }

    private void PlayFloorAnim()
    {
        floor.GetComponent<Animator>().SetTrigger("floor");
    }

    private void PlaySpeakerAnim()
    {
        animator.SetTrigger("pump");
    }

    private void SpawnSpeakerParticle()
    {
        Instantiate(speakerParticle, speaker.transform.TransformPoint(Vector3.zero), Quaternion.identity);
    }
}
