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
        IceBlock = 6
    };

    public enum Items
    {
        PlankItem = 25,
        FanItem = 26,
        FlagItem = 27,
        ActivatorItem = 28
    }

    public enum Effects
    {
        None,
        Freeze,
        Destroy,
        Activate
    };

    private Animator animator;

    public Animator _Animator { get => animator; }

    public abstract void Place();

    public abstract void Destroy();

    protected void RebindAnimator()
    {
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    public void SetAnimationTrigger(in string trigger)
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

    public bool IsBlock()
    {
        if (this.GetType() == typeof(Block))
        {
            return true;
        }
        return false;
    }

    public bool IsItem()
    {
        if (this.GetType() == typeof(Item))
        {
            return true;
        }
        return false;
    }

    public abstract new string ToString { get; }
}