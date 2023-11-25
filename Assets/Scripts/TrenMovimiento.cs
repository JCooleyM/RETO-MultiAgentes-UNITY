using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenMovimiento : MonoBehaviour
{
    float speed = 25f;
    Vector3 origen = new Vector3(0, 0, 0);
    Vector3 destino = new Vector3(0, 0, 65);
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            // Reached the current waypoint, move to the next one
            transform.position = origen;
        }
        else
        {
            float t = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destino, t);
        }
        
    }
}
