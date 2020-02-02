using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    private Animator animator=null;
    [SerializeField] GameObject inventoryMarker;
    [SerializeField] private GameObject animatedObject;

    public GameObject InventoryMarker { get => inventoryMarker;}

    public Animator GetAnimator()
    {
        if (animator == null)
        {
            animator = animatedObject.GetComponent<Animator>();
            return animator;
        }
        else
        {
            return animator;
        }
    }
}
