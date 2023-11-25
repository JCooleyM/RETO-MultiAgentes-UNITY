using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;
using System;

public class APIController : MonoBehaviour
{
    public readonly string API_URL = "http://localhost:3000";
    public int cars = 1000;
    public bool all_lanes = true;

    public GameObject Carro1;
    public GameObject Carro2;
    public GameObject Carro3;
    public GameObject Carro4;
    public GameObject Tren;

    private Quaternion rotation = Quaternion.Euler(0, 0, 0);
    private string simId;
    private List<JSONNode> simulacionSteps = new();
    private Dictionary<int, GameObject> carrosInstanciados = new();
    private int index = 0;

    private float stepStartTime;
    private float stepDuration = 0.5f;

    IEnumerator IniciarSimulacion()
    {
        // Llamar funcion de instancias iniciales
        string initialURL = API_URL + "/api/init_sim?cars=" + cars + "&all_lanes=" + all_lanes;

        UnityWebRequest InitialInfoRequest = UnityWebRequest.Get(initialURL);
        yield return InitialInfoRequest.SendWebRequest();
        
        // Manejar errores
        if (InitialInfoRequest.result == UnityWebRequest.Result.ConnectionError || InitialInfoRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(InitialInfoRequest.error);
            yield break;
        }
        
        // Instancias Carros en posicion inicial
        JSONNode simulation_Info = JSON.Parse(InitialInfoRequest.downloadHandler.text);

        // Obtenemos el id de la simulacion actual
        simId = simulation_Info["sim_id"]; 
        JSONNode instanciasIniciales = simulation_Info["data"]["car_initial_positions"];

        // Recorremos el json para obtener todas las posiciones iniciales de los carros
        for (int i = 0; i < instanciasIniciales.Count; i++)
        {
            Vector3 position = new Vector3(instanciasIniciales[i]["pos"]["x"], instanciasIniciales[i]["pos"]["y"],instanciasIniciales[i]["pos"]["z"]);
            int color = instanciasIniciales[i]["color"];
            GameObject instanciaCarro = null;

            // Dependiendo del color del carro instanciamos el modelo correpondiente
            switch (color)
            {
                case 1:
                    instanciaCarro = Instantiate(Carro1, position, rotation);
                    break;
                case 2:
                    instanciaCarro = Instantiate(Carro2, position, rotation);
                    break;
                case 3:
                    instanciaCarro = Instantiate(Carro3, position, rotation);
                    break;
                default:
                    instanciaCarro = Instantiate(Carro4, position, rotation);
                    break;
            }

            if (instanciaCarro != null)
            {
                int carId = instanciasIniciales[i]["id"].AsInt;
                carrosInstanciados[carId] = instanciaCarro;
            }
        }        

        // Instanciar tren si no se selecciones todos los carriles del tren
        if (!all_lanes)
        {
            Vector3 position = new Vector3(0, 0, 0);
            Instantiate(Tren, position, rotation);;
        }

        StartCoroutine(CorrerSimulacion());
    }
    
    IEnumerator CorrerSimulacion()
    {
        // Este while se encarga de obtener todos los steps de la simulacion por medio de la API
        while (true)
        {
            // Usamos el identificador de la simulacion
            string initialURL = API_URL + "/api/sim_step?sim_id=" + simId;

            UnityWebRequest InitialInfoRequest = UnityWebRequest.Get(initialURL);
            yield return InitialInfoRequest.SendWebRequest();

            // Manejar errores
            if (InitialInfoRequest.result == UnityWebRequest.Result.ConnectionError || InitialInfoRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(InitialInfoRequest.error);
                yield break;
            }

            JSONNode simulationInfo = JSON.Parse(InitialInfoRequest.downloadHandler.text);
            
            // CUando termine la simulacion nos salimos del while
            if (simulationInfo["message"].Value == "Simulation finished")
            {
                break;
            }

            // Agregamos los datos de cada step en un arreglo (Incluye los estados de lso semaforos y las posiciones de cada carro)
            simulacionSteps.Add(simulationInfo);
        }
    }

    //Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IniciarSimulacion());    
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= simulacionSteps.Count) return;

        // Procesa el paso actual
        JSONNode step = simulacionSteps[index]["data"]["car_positions"];

        for (int i = 0; i < step.Count; i++)
        {
            int carId = step[i]["id"].AsInt;

            GameObject carro = carrosInstanciados[carId];
            if (carro.TryGetComponent<CarMovement>(out var carMovement))
            {
                JSONNode pos = step[i]["pos"];
                Vector3 newPos = new Vector3(pos["x"], pos["y"], pos["z"]);

                carMovement.moveTowardsPosition(newPos);
            }
        }

        if (Time.time - stepStartTime >= stepDuration)
        {
            index++;
            stepStartTime = Time.time; // Reinicia el tiempo de inicio para el nuevo paso
        }
    }
}
