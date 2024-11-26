using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMG : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("HMG attack");
    }
}