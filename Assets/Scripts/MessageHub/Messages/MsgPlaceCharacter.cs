using UnityEngine;

public class MsgPlaceCharacter
{
    private Vector3 position;
    private Vector3 rotation;
    private Transform newParent;

    public MsgPlaceCharacter(Vector3 position, Vector3 rotation, Transform newParent)
    {
        this.position = position;
        this.rotation = rotation;
        this.newParent = newParent;
    }

    public Vector3 Position { get => position; }
    public Vector3 Rotation { get => rotation; }

    public Transform NewParent { get => newParent; }
}