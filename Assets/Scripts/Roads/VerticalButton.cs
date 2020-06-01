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

    private void Awake()
    {
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pressed " + buttonName);
        //Quito el mensaje y lo cambio a una referencia directa ya que es muy importante que se ejecuten en
        //el orden correcto
        //EventAggregator.Instance.Publish(new MsgAddInputFromButton(buttonName));
        GameLogic.Instance.AddInputFromButton(buttonName);

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