using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightColorChanger : MonoBehaviour
{
    public Material luzVerde;
    public Material luzRoja;

    public void ChangeColor(string color)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (color.Equals("green", System.StringComparison.OrdinalIgnoreCase))
        {
            renderer.material = luzVerde;
        }

        else if (color.Equals("red", System.StringComparison.OrdinalIgnoreCase))
        {
            renderer.material = luzRoja;
        }


    }


    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
