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
    public bool UseOnFrontBlock { get => useOnFrontBlock; set => useOnFrontBlock = value; }
    public bool UseOnFrontBelowBlock { get => useOnFrontBelowBlock; set => useOnFrontBelowBlock = value; }
    public bool UseOnPlayersHand { get => useOnPlayersHand; set => useOnPlayersHand = value; }
    public Vector3 FollowOffset { get => followOffset; set => followOffset = value; }

    [SerializeField] private bool pickable;
    [SerializeField] private bool parentToBlockParent;
    [SerializeField] private Effects effect;
    [SerializeField] private bool useOnFrontBlock;
    [SerializeField] private bool useOnFrontBelowBlock;
    [SerializeField] private bool useOnPlayersHand;
    [SerializeField] private bool inactiveOnUse = true;
    private Transform transformToFollow;
    private Vector3 followOffset;

    private void Update()
    {
        if (transformToFollow != null)
        {
            transform.position = Vector3.Lerp(transform.position, transformToFollow.position + followOffset, 1);
        }
    }

    public void Use()
    {
        transformToFollow = null;
        if (inactiveOnUse)
        {
            gameObject.SetActive(false);
        }
        else
        {
            SetAnimationTrigger("Use");
        }
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