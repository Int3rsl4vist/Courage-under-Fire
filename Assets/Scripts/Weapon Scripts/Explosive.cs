using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("Detonating explosive");
    }
}