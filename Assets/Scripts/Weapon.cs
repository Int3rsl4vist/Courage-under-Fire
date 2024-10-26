using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool isEquiped;
    // Start is called before the first frame update
    void Start()
    {
        Attack();
    }

    protected abstract void Attack();

    // Update is called once per frame
    void Update()
    {
        
    }
}
