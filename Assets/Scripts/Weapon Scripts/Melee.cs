using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("Melee attack");
    }
}
