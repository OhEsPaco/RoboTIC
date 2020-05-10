using UnityEngine;

public class GenericButton : MonoBehaviour
{
    public delegate void Clicked();
    private Clicked clickCalbacks;
    [SerializeField] private AudioClip buttonClick;
    private EventAggregator eventAggregator;

    public GameObject mesh;
    private Animation anim;
    private bool enable = true;

    public Clicked ClickCalbacks { get => clickCalbacks; set => clickCalbacks+= value; }

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
            clickCalbacks();
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