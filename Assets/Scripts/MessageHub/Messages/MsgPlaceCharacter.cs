using UnityEngine;

public class MsgPlaceCharacter
{
    private Vector3 position;
    private Vector3 rotation;

    public MsgPlaceCharacter(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public Vector3 Position { get => position; }
    public Vector3 Rotation { get => rotation; }
}