using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("RPG attack");
    }
}