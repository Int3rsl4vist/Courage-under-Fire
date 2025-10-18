using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    [Header("Object movement:")]
    [Description("Insert GameObject")] public Transform pointBTransform;
    [Description("How fast the obejct will move towards point B")] public float moveSpeed;
    [Description("Point B's coordinates")][SerializeField] Vector3 pointB;

    [Header("Wheel rotation:")]
    [Description("Rear Wheel axis (GameObject)")] public Transform axisA;
    [Description("Front Wheel axis (GameObject)")] public Transform axisB;
    [Description("How fast the wheels will rotate (Degrees per second)")] public float wheelRotationSpeed = 360f;

    private void Awake()
    {
        pointB = pointBTransform.position;
    }
    IEnumerator Start()
    {
        var pointA = transform.position;
        while (true)
        {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB, moveSpeed));
            yield return StartCoroutine(MoveObject(transform, pointB, pointA, moveSpeed));
        }
    }

    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        var i = 0.0f;
        var rate = 1.0f / time;
        bool movingToPointB = endPos == pointB;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);

            float wheelRotation = wheelRotationSpeed * Time.deltaTime;

            if (movingToPointB)
            {
                axisA.Rotate(Vector3.up, -wheelRotation);
                axisB.Rotate(Vector3.up, -wheelRotation);
            }
            else
            {
                axisA.Rotate(Vector3.up, wheelRotation);
                axisB.Rotate(Vector3.up, wheelRotation);
            }
            yield return null;
        }
    }
}