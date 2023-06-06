using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fer-ho singleton
public class MotorSimuladorScript : MonoBehaviour
{
    private int idSeguentObjecte;
    public float escalaTemps;
    private int entitatsTemporals = 0;
    private GameObject generadorPrefab;
    private GameObject cuaPrefab;
    private GameObject processadorPrefab;
    private GameObject sortidaPrefab;
    private List<GameObject> objectesLlibreria = new List<GameObject>();
    private int detallsObert = -1;
    private PriorityQueue<Esdeveniment> llistaEsdevenmients = new PriorityQueue<Esdeveniment>((a, b) => {
        int result = a.temps.CompareTo(b.temps);
        if (result != 0)
            return result;

        return a.tipusEsdeveniment.CompareTo(b.tipusEsdeveniment);
    });
    private float tempsActual = 0;
    private float tempsMaxim = 86400;

    public enum estats { ATURAT, SIMULANT };
    public estats estat = estats.ATURAT;

    private static MotorSimuladorScript instancia;

    private MotorSimuladorScript() { }

    public static MotorSimuladorScript Instancia {
        get {
            if (instancia == null){
                instancia = FindObjectOfType<MotorSimuladorScript>();

                if (instancia == null){
                    GameObject singletonObject = new GameObject();
                    instancia = singletonObject.AddComponent<MotorSimuladorScript>();
                    singletonObject.name = typeof(UIScript).ToString();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instancia;
        }
    }

    void Start()
    {
        generadorPrefab = Resources.Load("LlibreriaObjectes/Generador/Generador") as GameObject;
        cuaPrefab = Resources.Load("LlibreriaObjectes/Cua/Cua") as GameObject;
        processadorPrefab = Resources.Load("LlibreriaObjectes/Processador/Processador") as GameObject;
        sortidaPrefab = Resources.Load("LlibreriaObjectes/Sortida/Sortida") as GameObject;
        idSeguentObjecte = 0;
        tempsMaxim = 86400;
        estat = estats.ATURAT;
    }


    void Update()
    {
        if (UIScript.Instancia.ObteEstatSimulador() == 0){
            if (llistaEsdevenmients.Count > 0){
                Esdeveniment eActual = llistaEsdevenmients.Dequeue();
                tempsActual = eActual.temps;
                eActual.Executar();
                UIScript.Instancia.UltimEsdeveniment(eActual);
                for (int i = 0; i < transform.childCount; ++i) transform.GetChild(i).GetComponent<LlibreriaObjectes>().ActualizaEstadistics();
            }
            if (llistaEsdevenmients.Count == 0 && tempsMaxim > tempsActual) {
                tempsActual = tempsMaxim;
                for (int i = 0; i < transform.childCount; ++i) transform.GetChild(i).GetComponent<LlibreriaObjectes>().ActualizaEstadistics();
            }   
        }
    }

    public void IniciaSimulacio(){
        estat = estats.SIMULANT;
        for (int i = 0; i < transform.childCount; ++i) transform.GetChild(i).GetComponent<LlibreriaObjectes>().IniciaSimulacio();
    }

    public void ReiniciarSimulador(){
        estat = estats.ATURAT;
        llistaEsdevenmients = new PriorityQueue<Esdeveniment>((a, b) => {
            int result = a.temps.CompareTo(b.temps);
            if (result != 0)
                return result;

            return a.tipusEsdeveniment.CompareTo(b.tipusEsdeveniment);
        });
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("EntitatsTemporals");

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < transform.childCount; ++i){
            transform.GetChild(i).GetComponent<LlibreriaObjectes>().GenerarPlots();
            transform.GetChild(i).GetComponent<LlibreriaObjectes>().ReiniciaSimulador();
        }

        tempsActual = 0;
    }

    public float ObteTempsActual()
    {
        return tempsActual;
    }

    public void ExecutarSeguentEsdeveniment(){
        if (llistaEsdevenmients.Count == 0 && tempsActual == 0) IniciaSimulacio();
        else if (llistaEsdevenmients.Count > 0){
            Esdeveniment eActual = llistaEsdevenmients.Dequeue();
            tempsActual = eActual.temps;
            eActual.Executar();
            UIScript.Instancia.UltimEsdeveniment(eActual);
            for (int i = 0; i < transform.childCount; ++i) transform.GetChild(i).GetComponent<LlibreriaObjectes>().ActualizaEstadistics();
        }
        if (llistaEsdevenmients.Count == 0 && tempsMaxim > tempsActual) {
            tempsActual = tempsMaxim;
            for (int i = 0; i < transform.childCount; ++i) transform.GetChild(i).GetComponent<LlibreriaObjectes>().ActualizaEstadistics();
        }
    }

    public float ObteTempsSeguentEsdeveniment(){
        if (llistaEsdevenmients.Count > 0) return llistaEsdevenmients.FirstElement().temps;
        else return tempsActual;
    }

    public void AfegirEsdeveniment(Esdeveniment nouEsdeveminemt){
        if (nouEsdeveminemt.temps <= tempsMaxim) llistaEsdevenmients.Enqueue(nouEsdeveminemt);
    }

    public void AfegirObjecte(GameObject nouObjecte){
        if (!nouObjecte.name.Contains("Sortida")){
            if (detallsObert != -1) {
                transform.GetChild(detallsObert).GetComponent<LlibreriaObjectes>().TancaDetalls();
            }
            nouObjecte.GetComponent<LlibreriaObjectes>().ObreDetalls();
        }
        detallsObert = transform.childCount-1;
    }

    public void EliminarObjecte(GameObject objecte) {
        for (int i = 0; i < transform.childCount; ++i){
            transform.GetChild(i).GetComponent<LlibreriaObjectes>().IntentaEliminarObjecteSeguents(objecte);
            transform.GetChild(i).GetComponent<LlibreriaObjectes>().IntentaEliminarObjectePredecessor(objecte);
        }
        Destroy(objecte);
    }

    public bool AlgunDetallsObert(){
        return detallsObert != -1;
    }

    public void TancaDetallsObert(){
        if (AlgunDetallsObert()){
            transform.GetChild(detallsObert).GetComponent<LlibreriaObjectes>().TancaDetalls();
            detallsObert = -1;
        }
    }

    public void ObreDetallsFill(int nFill){
        transform.GetChild(nFill).GetComponent<LlibreriaObjectes>().ObreDetalls();
        detallsObert = nFill;
    }

    public bool RatoliSobreDetalls(){
        if (detallsObert == -1) return false;
        return transform.GetChild(detallsObert).GetComponent<LlibreriaObjectes>().RatoliSobreDetalls();
    }

    public void CreaObjecteFill(int obj, Vector3 posicio){
        GameObject objecteNou;
        switch(obj){
            case 0:
                objecteNou = Instantiate(generadorPrefab, new Vector3(posicio.x, posicio.y,0), Quaternion.identity);        
                break;
            case 1:
                objecteNou = Instantiate(cuaPrefab, new Vector3(posicio.x, posicio.y,0) , Quaternion.identity);
                break;
            case 2:
                objecteNou = Instantiate(processadorPrefab, new Vector3(posicio.x, posicio.y,0) , Quaternion.identity);
                break;
            case 3:
                objecteNou = Instantiate(sortidaPrefab, new Vector3(posicio.x, posicio.y,0), Quaternion.identity);
                break;
            default:
                objecteNou = Instantiate(generadorPrefab, new Vector3(posicio.x, posicio.y,0), Quaternion.identity);
                break;
        }
        objecteNou.transform.parent = gameObject.transform;
        AfegirObjecte(objecteNou);
    }

    public void CanviaEntitatsTemporals(int entitatsSeleccionades){
        entitatsTemporals = entitatsSeleccionades;
    }

    public void CanviaTempsMaxim(float tMax){
        tempsMaxim = tMax;
    }

    public int ObteEntitatsSeleccionades(){
        return entitatsTemporals;
    }

    public int ObteIdSeguentObjecte(){
        ++idSeguentObjecte;
        return idSeguentObjecte-1;
    }
}
