using UnityEngine;
using static ButtonConstants;

public class RoadButton : MonoBehaviour
{
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;
    public GameObject mesh;
    private LevelManager manager;
    private Animation anim;

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