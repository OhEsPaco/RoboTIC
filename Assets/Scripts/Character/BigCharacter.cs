using UnityEngine;

public class BigCharacter : Character
{
    [SerializeField] private GameObject inventoryMarker;

    public Vector3 GetInventoryPosition()
    {
        return inventoryMarker.transform.position;
    }
}