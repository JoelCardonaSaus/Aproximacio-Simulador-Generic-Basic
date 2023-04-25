using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuaScript : MonoBehaviour, IObjectes, ITractarEsdeveniment
{
    
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    public List<GameObject> SeguentsObjectes;
    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte = new Queue<GameObject>();
    private Dictionary<GameObject, double> tempsObjecteCua = new Dictionary<GameObject, double>();

    private enum states { EMPTY, NOEMPTY };

    private states state = states.EMPTY;
    private float timeEmpty = 0;
    private float timeNoEmpty = 0;
    private float timeScale = 1;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cuaObjecte.Count > 0){
            IEnumerator<GameObject> enumerator = cuaObjecte.GetEnumerator();
            while (enumerator.MoveNext()) {
                tempsObjecteCua[enumerator.Current] += (Time.deltaTime * timeScale);
            }
            sendObject();
            timeNoEmpty += (Time.deltaTime * timeScale);
        }
        else {
            timeEmpty += (Time.deltaTime * timeScale);
        }
        
    }

    public void generarEsdevenimentArribada(float tempsActual){
        if (cuaObjecte.Count != 0) {
            Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+1, Esdeveniment.Tipus.ARRIBADES);
            gameObject.parent.GetComponent<MotorDeSimulacio>().afegirEsdeveniment(e);
        }
    }

    public void TractarEsdeveniment(Esdeveniment e){
        generarEsdevenimentArribada(e.temps);
    }

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
    }

    public bool isAvailable()
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima) return true;
        else return false;
    }

    public void recieveObject(GameObject entity)
    {
        Debug.Log("La CUA rep un objecte!");
        entity.transform.position = transform.position + new Vector3(0,+1,0);
        if (state == states.EMPTY) state = states.NOEMPTY;
        cuaObjecte.Enqueue(entity);
        tempsObjecteCua.Add(entity, 0);

    }

    public bool sendObject(){
        IObjectes NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                foreach (GameObject objecte in SeguentsObjectes)
                {
                    NextObjecte = objecte.GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable()) {
                        NextObjecte.recieveObject(cuaObjecte.Dequeue());
                        if (cuaObjecte.Count == 0) state = states.EMPTY;
                        return true;
                    }
                }
                return false;
            }
            else if (enrutament == politiquesEnrutament.RANDOM){
                for (int i = 0; i < SeguentsObjectes.Count; i++){
                    int obj = Random.Range(0, SeguentsObjectes.Count);
                    NextObjecte = SeguentsObjectes[obj].GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable()) {
                        NextObjecte.recieveObject(cuaObjecte.Dequeue());
                        if (cuaObjecte.Count == 0) state = states.EMPTY;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private int nDisponibles(){
        int n = 0;
        foreach (GameObject seguent in SeguentsObjectes){
            if (seguent.GetComponent<IObjectes>().isAvailable()) ++n;
        }
        return n;
    }

    public int getState(){
        return (int)state;
    }

    public void OnMouseDown()
    {
        MotorSimuladorScript motorScript = gameObject.transform.parent.GetComponent<MotorSimuladorScript>();
        if (motorScript.AlgunDetallsObert())
        {
            motorScript.TancaDetallsObert();
        }
        motorScript.ObreDetallsFill(transform.GetSiblingIndex());
    }

    public void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool RatoliSobreDetalls(){
        var image = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    public void ActualitzaPropietats(politiquesEnrutament nouEnrutament, int capacitatMax){
        enrutament = nouEnrutament;
        capacitatMaxima = capacitatMax;
    }
}
