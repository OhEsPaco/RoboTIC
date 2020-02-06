using System;
using UnityEngine;

public class Block : LevelObject
{
    [SerializeField] private Blocks blockType;

    public Blocks BlockType { get => blockType; }

    public override string ToString { get => blockType.ToString() + " block"; }

    [System.Serializable]
    public class EffectReaction
    {
        public Effects effect;
        public Blocks block;
        //Posibilidad de usar metodo
        //Y de cambiar propiedades
    }

    [SerializeField] private EffectReaction effectReaction;
    [SerializeField] private BlockProperties[] blockProperties = new BlockProperties[0];

    public void Use()
    {
        SetTrigger(_Animator, "Use");
    }

    public override void Destroy()
    {
    }

    public override void Place()
    {
    }

    public override void Rebind()
    {
        RebindAnimator(_Animator);
    }
}