using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveDuration = 0.5f;
    private bool isMoving = false;

    public void moveTowardsPosition(Vector3 newPos)
    {
        if (!isMoving)
        {
            targetPosition = newPos;
            StartCoroutine(MoveToPosition(targetPosition, moveDuration));
        }
    }

    // Corutina para mover el objeto
    private IEnumerator MoveToPosition(Vector3 target, float duration)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    void Start()
    {
        // Inicializar con la posición actual para evitar movimientos no deseados
        targetPosition = transform.position;
    }
}
