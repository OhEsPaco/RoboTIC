using UnityEngine;

public class Item : LevelObject
{
    [SerializeField] private Items itemType;

    [Range(0f, 10f)]
    [SerializeField] private static float minimumPickDistance = 0.2f;

    public Items ItemType { get => itemType; }
    public bool Pickable { get => pickable; }

    public override string ToString { get => itemType.ToString() + " item"; }
    public Effects Effect { get => effect; }
    public bool ParentToBlockParent { get => parentToBlockParent; }

    [SerializeField] private bool pickable;
    [SerializeField] private bool parentToBlockParent;
    [SerializeField] private Effects effect;
    private Transform transformToFollow;
    private Vector3 followOffset;

    private void Update()
    {
        if (transformToFollow != null)
        {
            transform.position = Vector3.Lerp(transform.position, transformToFollow.position + followOffset, 0);
        }
    }

    public void Use()
    {
        SetAnimationTrigger("Use");
    }

    public void Pick(Transform transformToFollow, Vector3 followOffset)
    {
        this.transformToFollow = transformToFollow;
        this.followOffset = followOffset;
    }

    public override void Destroy()
    {
    }

    public override void Place()
    {
    }
}