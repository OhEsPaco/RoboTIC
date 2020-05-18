using UnityEngine;
using static LevelButtons;

public class MapEditorRoadButton : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClick;
    private EventAggregator eventAggregator;
    [SerializeField] private Buttons buttonType;

    public delegate void RoadButtonClicked(Buttons button, int nAvailableInstructions);

    private RoadButtonClicked rbc;

    private Animation anim;
    private bool enable = true;
    private Counter counter;

    public RoadButtonClicked Subscribe { set => rbc += value; }

    private void Awake()
    {
        try
        {
            anim = transform.Find("Mesh").GetComponent<Animation>();
        }
        catch
        {
            anim = GetComponent<Animation>();
        }

        counter = GetComponentInChildren<Counter>();
        eventAggregator = EventAggregator.instance;
    }

    private void OnMouseDown()
    {
        if (enable)
        {
            if (counter != null)
            {
                counter.SetNumber(counter.ActualNumber + 1);
                rbc(buttonType, counter.ActualNumber);
            }
            else
            {
                rbc(buttonType, 0);
            }

            if (anim != null)
            {
                if (eventAggregator != null)
                {
                    eventAggregator.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(buttonClick, 1.0f, transform.position));
                }
                if (anim.isPlaying)
                {
                    return;
                }
                else
                {
                    anim.Play("ButtonPressed");
                }
            }
        }
    }

    public void Disable()
    {
        enable = false;
    }

    public void Enable()
    {
        enable = true;
    }
}