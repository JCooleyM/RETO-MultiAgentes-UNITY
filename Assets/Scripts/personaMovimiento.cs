using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class personaMovimiento : MonoBehaviour
{

    public Vector3 origen = new Vector3(0, 0, 0);
    public Vector3 destino = new Vector3(0, 0, 0);
    float speed = 2f;
    float rotationSpeed = .3f;
    int cont = 40;
    

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            transform.position = origen;
        }
        else
        {
            float t = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destino, t);

            transform.Rotate(rotationSpeed, 0, 0, Space.Self);
            cont++;
        }

        if (cont > 80) 
        {
            rotationSpeed = rotationSpeed * -1;
            cont = 0;
        }
    }
}
