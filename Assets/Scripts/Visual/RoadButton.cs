using UnityEngine;
using static LevelButtons;

public class RoadButton : MonoBehaviour
{
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;
    public GameObject mesh;
    private LevelManager manager;
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
        manager = LevelManager.instance;
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }

    private void OnMouseDown()
    {
        if (enable)
        {
            Debug.Log("Pressed " + buttonIndex.ToString("g"));
            manager.RoadPlacementLogic.AddInputFromButton(buttonIndex);
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