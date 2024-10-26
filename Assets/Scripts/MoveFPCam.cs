using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFPCam : MonoBehaviour
{
    public Transform camPosition;
    internal void Update()
    {
        transform.position = camPosition.position;
    }
}
