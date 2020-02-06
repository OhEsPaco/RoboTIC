using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
    //Como guardo el nivel en un solo array, del 0 al 24 son bloques y del 25 al infinito items

    public enum Blocks
    {
        NoBlock = 0,
        WaterBlock = 1,
        LavaBlock = 2,
        SolidBlock = 3,
        LiftBlock = 4,
        SpikesBlock = 5,
        IceBlock = 6,
        LiftBlockActivated = 7,
        SpikesBlockActivated = 8
    };

    public enum Items
    {
        PlankItem = 25,
        FanItem = 26,
        FlagItem=27
    }

    public enum Effects
    {
        None,
        Freeze,
        Destroy,
        Activate
    };

    public enum BlockProperties
    {
        Immaterial,
        Walkable,
        Dangerous,
        Icy
    }

    private Animator animator;

    public Animator _Animator { get => animator; }

    public abstract void Place();

    public abstract void Destroy();

    public abstract void Rebind();

    protected void RebindAnimator(Animator animator)
    {
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    protected void SetTrigger(Animator animator, in string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    public void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator not set in " + ToString);
        }
    }

    public abstract new string ToString { get; }
}