using UnityEngine;
using static EditorLogic;

public class EditorObject
{
    private GameObject associatedGameobject;
    private EditorToolType objectType;
    private int objectIdentifier;

    public EditorObject(GameObject associatedGameobject, EditorToolType objectType, int objectIdentifier)
    {
        this.associatedGameobject = associatedGameobject;
        this.objectType = objectType;
        this.objectIdentifier = objectIdentifier;
    }

    public GameObject AssociatedGameobject { get => associatedGameobject; set => associatedGameobject = value; }
    public EditorToolType ObjectType { get => objectType; set => objectType = value; }
    public int ObjectIdentifier { get => objectIdentifier; set => objectIdentifier = value; }
}