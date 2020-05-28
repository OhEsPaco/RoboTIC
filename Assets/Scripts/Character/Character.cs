using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public Animator _Animator { get => animator; }

    public void RebindAnimator()
    {
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    protected void SetAnimationTrigger(in string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}