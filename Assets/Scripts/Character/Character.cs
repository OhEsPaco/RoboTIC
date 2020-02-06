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

    protected void SetAnimationTrigger(in string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    public void Start()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator not set in character");
        }
    }
}