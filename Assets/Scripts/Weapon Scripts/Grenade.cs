using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("Throwing grenade");
    }
}