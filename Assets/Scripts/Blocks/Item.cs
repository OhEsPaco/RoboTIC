using UnityEngine;

public class Item : LevelObject
{
    [SerializeField] private Items itemType;

    public Items ItemType { get => itemType; }
    public bool Pickable { get => pickable; }

    public override string ToString { get => itemType.ToString() + " item"; }
    public Effects Effect { get => effect; }
    public bool ParentToBlockParent { get => parentToBlockParent;}

    [SerializeField] private bool pickable;
    [SerializeField] private bool parentToBlockParent;
    [SerializeField] private Effects effect;

    public void Use()
    {
        SetAnimationTrigger("Use");
    }

    public void Pick()
    {
    }

    public override void Destroy()
    {
    }

    public override void Place()
    {
    }
}