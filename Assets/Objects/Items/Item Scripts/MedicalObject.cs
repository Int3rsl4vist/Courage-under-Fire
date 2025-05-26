using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Medical Object", menuName = "Inventory System/Items/Medical")]
public class MedicalObject : ItemObject
{
    public int restoreHealth;
    public void Awake()
    {
        type = ItemType.Medical;
    }
}
