using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectConstants;

public class Item : MonoBehaviour
{
    [SerializeField] ObjectType objectType;

    public ObjectType ObjectType { get => objectType; }
}
