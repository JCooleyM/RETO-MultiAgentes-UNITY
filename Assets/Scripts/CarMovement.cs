using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 3.0f;

    public void moveTowardsPosition(Vector3 newPos)
    {
        targetPosition = newPos;
    }

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        transform.position = targetPosition;
    }
}
