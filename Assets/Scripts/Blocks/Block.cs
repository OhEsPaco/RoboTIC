using System;
using System.Collections.Generic;
using UnityEngine;

public class Block : LevelObject
{
    [SerializeField] private Blocks blockType;

    public Blocks BlockType { get => blockType; }

    public enum BlockActions
    {
        Use, Destroy, Place, Activate, Rebind
    }

    public enum BlockProperties
    {
        Immaterial,
        Walkable,
        Dangerous,
        Icy
    }

    public override string ToString { get => blockType.ToString() + " block"; }
    public BlockProperties[] _BlockProperties { get => blockProperties; set => blockProperties = value; }
    public EffectReaction[] EffectReactions { get => effectReaction; set => effectReaction = value; }

    [System.Serializable]
    public class EffectReaction
    {
        //Si hay 0 compatible items se activara con cualquiera
        public Items[] compatibleItems = new Items[0];

        public Effects effect;
        public bool replaceBlock = false;
        public Blocks block;
        public BlockActions[] actionsToExecute = new BlockActions[0];
        public BlockProperties[] newProperties = new BlockProperties[0];
        public string[] animationTriggers = new string[0];
    }

    [SerializeField] private BlockProperties[] blockProperties = new BlockProperties[0];
    [SerializeField] private EffectReaction[] effectReaction = new EffectReaction[0];
    private Dictionary<BlockActions, Action> actionsDictionary;

    private void Awake()
    {
        actionsDictionary = new Dictionary<BlockActions, Action>();
        actionsDictionary.Add(BlockActions.Use, Use);
        actionsDictionary.Add(BlockActions.Destroy, Destroy);
        actionsDictionary.Add(BlockActions.Place, Place);
        actionsDictionary.Add(BlockActions.Activate, Activate);
        actionsDictionary.Add(BlockActions.Rebind, RebindAnimator);
    }

    public void ExecuteAction(BlockActions action)
    {
        //quiza crear cola de acciones y controlar que terminen para pasar a la siguiente
        if (actionsDictionary.ContainsKey(action))
        {
            actionsDictionary[action]();
        }
        else
        {
            Debug.LogWarning("Unbinded BlockAction: " + action.ToString());
        }
    }

    public bool ActionsDone()
    {
        //COMPROBAR QUE SE HAYAN TERMINADO LAS ACCIONES
        return true;
    }

    public void Use()
    {
        SetAnimationTrigger("Use");
    }

    public override void Destroy()
    {
    }

    public override void Place()
    {
    }

    public void Activate()
    {
    }

    public bool CheckProperty(BlockProperties property)
    {
        foreach (BlockProperties prop in blockProperties)
        {
            if (prop == property)
            {
                return true;
            }
        }

        return false;
    }
}