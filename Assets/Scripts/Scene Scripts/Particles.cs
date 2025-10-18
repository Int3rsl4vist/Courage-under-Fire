using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Awake()
    {
        transform.position = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
