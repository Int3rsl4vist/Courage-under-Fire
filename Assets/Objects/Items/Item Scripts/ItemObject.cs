using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Ammo,
    Armor,
    Equipment,
    Medical,
    Weapon,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;
    [TextArea(5,20)]
    public string description;
}
