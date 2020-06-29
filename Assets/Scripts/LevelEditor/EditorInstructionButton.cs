using UnityEngine;
using static LevelButtons;

public class EditorInstructionButton : MonoBehaviour
{
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private Counter counter;

    [SerializeField] private GameObject mesh;
    private Animation anim;

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
        EventAggregator.Instance.Subscribe<MsgEditorResetAllCounters>(ResetCounter);
    }

    public void ResetCounter(MsgEditorResetAllCounters msg)
    {
        if (counter != null)
        {
            counter.SetNumber(0);
        }
    }

    public void OnSelect()
    {
        if (counter != null)
        {
            counter.SetNumber(counter.ActualNumber + 1);
            EventAggregator.Instance.Publish<MsgEditorAvailableInstructionsChanged>(new MsgEditorAvailableInstructionsChanged(buttonIndex, counter.ActualNumber));

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

            if (buttonClick != null)
            {
                EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(buttonClick, 1.0f, transform.position));
            }
        }
    }
}