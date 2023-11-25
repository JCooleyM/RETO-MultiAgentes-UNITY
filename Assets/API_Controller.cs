using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class API_Controller : MonoBehaviour
{
    public readonly string API_URL = "http://localhost:3000";
    public int cars;
    public bool all_lanes;

    public GameObject Carro1;
    public GameObject Carro2;
    public GameObject Carro3;
    public GameObject Carro4;
    public GameObject Tren;
    public Quaternion rotation = Quaternion.Euler(0, 0, 0);

    public string sim_id;
    public JSONNode instancias_iniciales;

    private List<JSONNode> simulacion_completa = new List<JSONNode>();



    IEnumerator Iniciar_Simulacion()
    {
        //Llamar funcion de instancias iniciales
        string initialURL = API_URL + "/api/init_sim?cars=" + cars + "&all_lanes=" + all_lanes;

        UnityWebRequest InitialInfoRequest = UnityWebRequest.Get(initialURL);

        yield return InitialInfoRequest.SendWebRequest();

        if (InitialInfoRequest.isNetworkError || InitialInfoRequest.isHttpError)
        {
            Debug.LogError(InitialInfoRequest.error);
            yield break;
        }

        
        // Instancias Carros en posicion inicial
        JSONNode simulation_Info = JSON.Parse(InitialInfoRequest.downloadHandler.text);

        sim_id = simulation_Info["sim_id"]; 

        instancias_iniciales = simulation_Info["data"]["car_initial_positions"];

        for (int i = 0; i < instancias_iniciales.Count; i++)
        {
            Vector3 position = new Vector3(instancias_iniciales[i]["pos"]["x"], instancias_iniciales[i]["pos"]["y"],instancias_iniciales[i]["pos"]["z"]);
            int color = instancias_iniciales[i]["color"];
            GameObject inst;
            
            switch (color)
            {
                case 1:
                    inst = Instantiate(Carro1, position, rotation);
                    break;

                case 2:
                    inst = Instantiate(Carro2, position, rotation);
                    break;

                case 3:
                    inst = Instantiate(Carro3, position, rotation);
                    break;

                default:
                    inst = Instantiate(Carro4, position, rotation);
                    break;
            }
        }        

        //Instanciar tren
        if (!all_lanes)
        {
            Vector3 position = new Vector3(0, 0, 0);
            GameObject inst = Instantiate(Tren, position, rotation);;
        }
    }

    
    

    
    IEnumerator Correr_Simulacion()
    {
        while (true)
        {
            string initialURL = API_URL + "/api/sim_step?sim_id=" + sim_id;

            UnityWebRequest InitialInfoRequest = UnityWebRequest.Get(initialURL);

            yield return InitialInfoRequest.SendWebRequest();

            if (InitialInfoRequest.isNetworkError || InitialInfoRequest.isHttpError)
            {
                Debug.LogError(InitialInfoRequest.error);
                yield break;
            }

            JSONNode simulation_Info = JSON.Parse(InitialInfoRequest.downloadHandler.text);

            if (simulation_Info["data"]["message"].Value == "Simulation finished")
            {
                break;
            }

            //Debug.Log(simulation_Info);
            simulacion_completa.Add(simulation_Info);

        }
    }


    //Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Iniciar_Simulacion());
        StartCoroutine(Correr_Simulacion());        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
