using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : Weapon
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("Handgun attack");
    }
}