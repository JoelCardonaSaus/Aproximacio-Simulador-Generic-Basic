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
    private PriorityQueue<Esdeveniment> llistaEsdevenmients = new PriorityQueue<Esdeveniment>((a, b) => a.temps.CompareTo(b.temps));
    private float tempsActual;
    // Start is called before the first frame update
    void Start()
    {
        generadorPrefab = Resources.Load("LlibreriaObjectes/Generador/Generador") as GameObject;
        cuaPrefab = Resources.Load("LlibreriaObjectes/Cua/Cua") as GameObject;
        processadorPrefab = Resources.Load("LlibreriaObjectes/Processador/Processador") as GameObject;
        sortidaPrefab = Resources.Load("LlibreriaObjectes/Sortida/Sortida") as GameObject;
        idSeguentObjecte = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIScript.Instancia.ObteEstatSimulador() == 0){
            if (llistaEsdevenmients.Count > 0){
                Esdeveniment eActual = llistaEsdevenmients.Dequeue();
                tempsActual = eActual.temps;
                eActual.Executar();
            }
        }
    }

    public void IniciaSimulacio(){
        for (int i = 0; i < objectesLlibreria.Count; i++){
            objectesLlibreria[i].GetComponent<LlibreriaObjectes>().IniciaSimulacio();
        }
    }

    public void ReiniciarSimulador(){
        llistaEsdevenmients = new PriorityQueue<Esdeveniment>((a, b) => a.temps.CompareTo(b.temps));
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("EntitatsTemporals");

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < objectesLlibreria.Count; i++){
            objectesLlibreria[i].GetComponent<LlibreriaObjectes>().GenerarPlots();
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
        }
    }

    public float ObteTempsSeguentEsdeveniment(){
        if (llistaEsdevenmients.Count > 0) return llistaEsdevenmients.FirstElement().temps;
        else return tempsActual;
    }

    public void AfegirEsdeveniment(Esdeveniment nouEsdeveminemt){
        llistaEsdevenmients.Enqueue(nouEsdeveminemt);
    }

    public void AfegirObjecteLlista(GameObject nouObjecte){
        if (!nouObjecte.name.Contains("Sortida")){
            if (detallsObert != -1) {
                objectesLlibreria[detallsObert].GetComponent<LlibreriaObjectes>().TancaDetalls();
            }
            nouObjecte.GetComponent<LlibreriaObjectes>().ObreDetalls();
        }
        objectesLlibreria.Add(nouObjecte);
        detallsObert = objectesLlibreria.Count-1;
    }

    public void EliminarObjecteLlista(GameObject objecte) {
        for (int i = 0; i < objectesLlibreria.Count; i++) {
            objectesLlibreria[i].GetComponent<LlibreriaObjectes>().IntentaEliminarObjecteSeguents(objecte);
        }
        objectesLlibreria.Remove(objecte);
        Destroy(objecte);
    }

    public bool AlgunDetallsObert(){
        return detallsObert != -1;
    }

    public void TancaDetallsObert(){
        if (AlgunDetallsObert()){
            objectesLlibreria[detallsObert].GetComponent<LlibreriaObjectes>().TancaDetalls();
            detallsObert = -1;
        }
    }

    public void ObreDetallsFill(int nFill){
        objectesLlibreria[nFill].GetComponent<LlibreriaObjectes>().ObreDetalls();
        detallsObert = nFill;
    }

    public bool RatoliSobreDetalls(){
        if (detallsObert == -1) return false;
        return objectesLlibreria[detallsObert].GetComponent<LlibreriaObjectes>().RatoliSobreDetalls();
    }
    private void OnValidate() {
        if (escalaTemps < 0.5f) escalaTemps = 0.5f;
        else if (escalaTemps > 10f) escalaTemps = 10f;     
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
        AfegirObjecteLlista(objecteNou);
    }

    public void CanviaEntitatsTemporals(int entitatsSeleccionades){
        entitatsTemporals = entitatsSeleccionades;
    }

    public int ObteEntitatsSeleccionades(){
        return entitatsTemporals;
    }

    public int ObteIdSeguentObjecte(){
        ++idSeguentObjecte;
        return idSeguentObjecte-1;
    }
}
