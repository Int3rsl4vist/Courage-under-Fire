using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    public override void Attack()
    {
        // Do damage to target enemy withing 1.5 m
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && isEquipped)
        {
            Attack();
        }
    }

}
