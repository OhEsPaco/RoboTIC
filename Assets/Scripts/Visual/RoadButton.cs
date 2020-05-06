using UnityEngine;
using static LevelButtons;

public class RoadButton : MonoBehaviour
{
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;
    [SerializeField] private AudioClip buttonClick;
    private EventAggregator eventAggregator;

    public GameObject mesh;
    private Animation anim;
    private bool enable = true;

    public Buttons ButtonType
    {
        get
        {
            return buttonIndex;
        }
    }

    private void Awake()
    {
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }

        eventAggregator = EventAggregator.instance;
    }

    private void OnMouseDown()
    {
        if (enable)
        {
            Debug.Log("Pressed " + buttonIndex.ToString("g"));

            EventAggregator.instance.Publish(new MsgAddInputFromButtonRoadPlacement(buttonIndex));
            if (mesh != null)
            {
                if (anim.isPlaying)
                {
                    return;
                }
                else
                {
                    anim.Play("ButtonPressed");
                }
            }

            if (eventAggregator != null)
            {
                eventAggregator.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(buttonClick, 1.0f, transform.position));
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