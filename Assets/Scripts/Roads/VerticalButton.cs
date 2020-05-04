using UnityEngine;
using static LevelButtons;

public class VerticalButton : MonoBehaviour
{
    [SerializeField] private GameObject mesh;
    [SerializeField] private Buttons buttonName;
    private Animation anim;
    private bool locked = false;

    public bool Locked { get { return locked; } }
    public string ButtonName { get { return buttonName.ToString(); } }

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void Awake()
    {
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pressed " + buttonName);

        EventAggregator.instance.Publish(new MsgAddInputFromButton(buttonName));
        //Send message here
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

    public void Lock()
    {
        this.locked = true;
    }

    public void Unlock()
    {
        this.locked = false;
    }
}