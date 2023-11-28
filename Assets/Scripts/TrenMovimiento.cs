using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenMovimiento : MonoBehaviour
{
    float speed = 10f;
    Vector3 origen = new Vector3(0, 0, 0);
    Vector3 destino = new Vector3(0, 0, 70);
    Vector3 position = new Vector3(0, -0.14f, 30);
    private Quaternion rotation = Quaternion.Euler(0, 90, 0);
    public GameObject Vias;
    


    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Vias, position, rotation);
    }

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
        }
        
    }
}
