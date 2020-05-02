using UnityEngine;

public class BigCharacter : Character
{
    [SerializeField] private GameObject inventoryMarker;
    /*  [SerializeField] private Animator animator;*/

    public Vector3 GetInventoryPosition()
    {
        return inventoryMarker.transform.position;
    }
}