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

    public TextMeshProUGUI ResultText;
    public GameObject Carro1;
    public GameObject Carro2;
    public GameObject Carro3;
    public GameObject Carro4;
    public GameObject Tren;
    public GameObject Sem_1_Main;
    public GameObject Sem_1_Entry;
    public GameObject Sem_2_Main;
    public GameObject Sem_2_Entry;


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
        
        // Instanciar luzes de semaforo
        Vector3 sem1MainPosition = new Vector3(4.5f, 2, 21.8f);
        Sem_1_Main = Instantiate(Sem_1_Main, sem1MainPosition, Quaternion.identity);

        Vector3 sem1EntryPosition = new Vector3(4.7f, 2, 22f);
        Sem_1_Entry = Instantiate(Sem_1_Entry, sem1EntryPosition, Quaternion.identity);

        Vector3 sem2MainPosition = new Vector3(4.5f, 2, 41.8f);
        Sem_2_Main = Instantiate(Sem_2_Main, sem2MainPosition, Quaternion.identity);
        
        Vector3 sem2EntryPosition = new Vector3(4.7f, 2, 42f);
        Sem_2_Entry = Instantiate(Sem_2_Entry, sem2EntryPosition, Quaternion.identity);

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
            
            // Cuando termine la simulacion nos salimos del while
            if (simulationInfo["message"].Value == "Simulation finished")
            {
                // Cambiar el texto para desplegar los resultados.
                ResultText.text = "Steps Totales: " + simulationInfo["total_steps"] + "\nCarros/step: " + Math.Round(simulationInfo["cars_per_step"].AsFloat, 3);
                break;
            }

            // Agregamos los datos de cada step en un arreglo (Incluye los estados de lso semaforos y las posiciones de cada carro)
            simulacionSteps.Add(simulationInfo);
        }
    }

    //Start is called before the first frame update
    void Start()
    {
        ResultText.text = "";
        StartCoroutine(IniciarSimulacion());    
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= simulacionSteps.Count) return;

        // Procesa el paso actual
        JSONNode step = simulacionSteps[index]["data"]["car_positions"];
        JSONNode trafficLights = simulacionSteps[index]["data"]["Traffic_Lights"];

        string Sem_1_Main_Color = trafficLights[0]["main_lane_state"];
        string Sem_1_Entry_Color = trafficLights[0]["entry_lane_state"];
        string Sem_2_Main_Color = trafficLights[1]["main_lane_state"];
        string Sem_2_Entry_Color = trafficLights[1]["entry_lane_state"];

        if (Sem_1_Main.TryGetComponent<TrafficLightColorChanger>(out var colorChanger1))
        {
            colorChanger1.ChangeColor(Sem_1_Main_Color);
        }

        if (Sem_1_Entry.TryGetComponent<TrafficLightColorChanger>(out var colorChanger2))
        {
            colorChanger2.ChangeColor(Sem_1_Entry_Color);
        }

        if (Sem_2_Main.TryGetComponent<TrafficLightColorChanger>(out var colorChanger3))
        {
            colorChanger3.ChangeColor(Sem_2_Main_Color);
        }

        if (Sem_2_Entry.TryGetComponent<TrafficLightColorChanger>(out var colorChanger4))
        {
            colorChanger4.ChangeColor(Sem_2_Entry_Color);
        }

        
        
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
