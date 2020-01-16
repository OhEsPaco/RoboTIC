using UnityEngine;
using static ButtonConstants;

public class VerticalButton : MonoBehaviour
{
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;
    [SerializeField] private GameObject mesh;
    private Animation anim;

    public Buttons ButtonIndex { get => buttonIndex; }

    
    private void Awake()
    {
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pressed " + buttonIndex.ToString("g"));
        LevelManager.instance.Logic.AddInputFromButton(buttonIndex);
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