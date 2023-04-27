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
    private Queue<GameObject> objectesRebutjats = new Queue<GameObject>();
    private List<GameObject> objectesPendentsPerRebre = new List<GameObject>();

    private enum states { BUIT, NOBUIT, BLOQUEJAT };
    private states estat = states.BUIT;

    private float timeEmpty = 0;
    private float timeNoEmpty = 0;
    private float timeScale = 1;


    void Start()
    {
        
    }

    void Update()
    {

    }

    public void TractarEsdeveniment(Esdeveniment e){
        switch (e.tipusEsdeveniment)
        {
            // Arribades a la cua
            case Esdeveniment.Tipus.ARRIBADES:
                if (objectesPendentsPerRebre.Contains(e.obteProductor())){
                    objectesPendentsPerRebre.Remove(e.obteProductor()); // Aquest objecte ja no ens te pendent per enviar cap objecte
                    if (estat == states.BUIT)
                    {
                        e.ObteEntitatImplicada().transform.position = transform.position + new Vector3(0,+1,0);    
                        int seguentObjecteEnviar = sendObject();
                        if (seguentObjecteEnviar != -1){ // Podem enviar directament a algun dels seguents objectes
                            tempsObjecteCua.Add(e.ObteEntitatImplicada(), 1);
                            Esdeveniment eNou = new Esdeveniment(this.gameObject, SeguentsObjectes[seguentObjecteEnviar], e.temps+1, e.ObteEntitatImplicada(), Esdeveniment.Tipus.ARRIBADES);
                            transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(eNou);
                        } else {
                            cuaObjecte.Enqueue(e.ObteEntitatImplicada());
                            tempsObjecteCua.Add(e.ObteEntitatImplicada(), e.temps);
                            estat = states.NOBUIT;
                        }
                    }       
                    else if (estat == states.NOBUIT){
                        cuaObjecte.Enqueue(e.ObteEntitatImplicada());
                        tempsObjecteCua.Add(e.ObteEntitatImplicada(), e.temps);
                    }
                } 
                break;
            case Esdeveniment.Tipus.PROCESSOS:
                break;
        }
    }

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
    }

    public bool isAvailable(GameObject objectePropietari)
    {
        if (objectesPendentsPerRebre.Count < capacitatMaxima && (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima)){
            objectesPendentsPerRebre.Add(objectePropietari);
            return true;
        } 
        else{
            if (!objectesRebutjats.Contains(objectePropietari))  objectesRebutjats.Enqueue(objectePropietari);
            return false;
        }
    }

    // L'objecte (entity) del parametre ens demana que li enviem una entitat de la cua
    public bool recieveObject(GameObject entity, float tempsActual)
    {
        if (estat != states.BUIT && entity.GetComponent<IObjectes>().isAvailable(this.gameObject)){
            Esdeveniment e = new Esdeveniment(this.gameObject, entity, tempsActual, cuaObjecte.Dequeue(), Esdeveniment.Tipus.ARRIBADES);
            transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
            if (cuaObjecte.Count == 0) estat = states.BUIT;

            // Busquem si alguns dels seguents objectes que hem rebutjat anteriorment ens pot enviar una entitat
            while (objectesRebutjats.Count != 0 && !objectesRebutjats.Dequeue().GetComponent<IObjectes>().recieveObject(this.gameObject, tempsActual)) return true;                
        }
        else if (estat == states.BUIT) return false;
        return false;
    }

    public int sendObject(){
        IObjectes NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                for (int i = 0; i < SeguentsObjectes.Count; i++)
                {
                    NextObjecte = SeguentsObjectes[i].GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable(this.gameObject)) {
                        if (cuaObjecte.Count == 0) estat = states.BUIT;
                        return i;
                    }
                }
            }
            else if (enrutament == politiquesEnrutament.RANDOM){
                for (int i = 0; i < SeguentsObjectes.Count; i++){
                    int obj = Random.Range(0, SeguentsObjectes.Count);
                    NextObjecte = SeguentsObjectes[obj].GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable(this.gameObject)) {
                        if (cuaObjecte.Count == 0) estat = states.BUIT;
                        return i;
                    }
                }
            }
        }
        return -1;
    }

    private int nDisponibles(){
        int n = 0;
        foreach (GameObject seguent in SeguentsObjectes){
            if (seguent.GetComponent<IObjectes>().isAvailable(this.gameObject)) ++n;
        }
        return n;
    }

    public int getState(){
        return (int)estat;
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

    public int ObteTipusObjecte()
    {
        return 1;
    }

    public void ActualitzaPropietats(politiquesEnrutament nouEnrutament, int capacitatMax){
        enrutament = nouEnrutament;
        capacitatMaxima = capacitatMax;
    }
}
