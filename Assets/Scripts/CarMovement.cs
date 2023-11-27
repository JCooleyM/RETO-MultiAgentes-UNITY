using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveDuration = 0.5f;
    private float rotationDuration = .5f;
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
        // Inicializar con la posici�n actual para evitar movimientos no deseados
        targetPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto colisionado tiene la etiqueta "Rotator"
        if (other.CompareTag("SalidaTunel"))
        {
            // Rotar 90 grados en el eje Y
            transform.Rotate(0, -90, 0);
        }

        else if (other.CompareTag("EntradaAvenida"))
        {
            StartCoroutine(RotacionGradual(90, rotationDuration));
        }
    }

     private IEnumerator RotacionGradual(float angulo, float duracion)
    {
        Quaternion iniciarRotacion = transform.rotation;
        Quaternion finRotacion = Quaternion.Euler(0, angulo, 0) * iniciarRotacion;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            transform.rotation = Quaternion.Slerp(iniciarRotacion, finRotacion, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.rotation = finRotacion;
    }
}
