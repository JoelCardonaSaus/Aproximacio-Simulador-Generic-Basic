using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuaScript : MonoBehaviour, IObjectes
{
    
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    public List<GameObject> SeguentsObjectes;
    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte;
    private Dictionary<GameObject, double> tempsObjecteCua;
    private Queue<GameObject> objectesRebutjats;
    private enum states { BUIT, NOBUIT, PLE };
    private states estat;


    void Start()
    {
    }

    void Update()
    {
    }

    public void IniciaSimulacio(){
        estat = states.BUIT;
        objectesRebutjats = new Queue<GameObject>();
        tempsObjecteCua = new Dictionary<GameObject, double>();
        cuaObjecte = new Queue<GameObject>();
    }

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) {
            Destroy(transform.GetChild(SeguentsObjectes.IndexOf(objecte)+1).gameObject);
            SeguentsObjectes.Remove(objecte);
        }
    }

    public void desajuntarSeguentObjecte(GameObject desjuntar){
        intentaEliminarObjecteSeguents(desjuntar);
    }

    public bool estaDisponible(GameObject objecteLlibreria)
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima){
            return true;
        } 
        else{
            if (!objectesRebutjats.Contains(objecteLlibreria))  objectesRebutjats.Enqueue(objecteLlibreria);
            return false;
        }
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == states.NOBUIT){
            cuaObjecte.Enqueue(entitat);
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsObjecteCua.Add(entitat, tActual);
            if (capacitatMaxima != -1 && cuaObjecte.Count < capacitatMaxima) estat = states.NOBUIT;
            else estat = states.PLE;
        }
        else if (estat == states.BUIT){
            int nDisponible = cercaDisponible();
            if (nDisponible != -1){
                SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
                tempsObjecteCua.Add(entitat, 0);
            } else {
                cuaObjecte.Enqueue(entitat);
                float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
                tempsObjecteCua.Add(entitat, tActual);
                if (capacitatMaxima!=-1 && capacitatMaxima > 1) estat = states.NOBUIT;
                else estat = states.PLE;
            }
        }
    }

    public int cercaDisponible(){   
        IObjectes SeguentObj;

        // Comprovem que almenys hi ha un objecte disponible
        if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
            for (int i = 0; i < SeguentsObjectes.Count; i++)//GameObject objecte in SeguentsObjectes)
            {
                SeguentObj = SeguentsObjectes[i].GetComponent<IObjectes>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return i;
                }
            }
        }

        else if (enrutament == politiquesEnrutament.RANDOM){
            for (int i = 0; i < SeguentsObjectes.Count; i++){
                int obj = Random.Range(0, SeguentsObjectes.Count);
                SeguentObj = SeguentsObjectes[obj].GetComponent<IObjectes>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return obj;
                }
            }
        }
        return -1;
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public bool notificacioDisponible(GameObject objecteLlibreria)
    {
        if (estat == states.BUIT) return false;
        else if (estat == states.NOBUIT){
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsObjecteCua[entitat] = tempsCua;
            objecteLlibreria.GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);


            if (cuaObjecte.Count != 0){
                int nDisponible = cercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
                    while (objectesRebutjats.Count != 0) {
                        // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                        if (AvisaDisponibilitat()) {
                            break;
                        }
                    }
                    estat = states.NOBUIT;
                } else {
                    estat = states.NOBUIT;
                }
            } else {
                while (objectesRebutjats.Count != 0) {
                    // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                    if (AvisaDisponibilitat()) {
                        break;
                    }
                }
                estat = states.BUIT;
            }
            return true; // En estat NOBUIT, si ens arriba una notificacioDisponibilitat sempre podrem enviar almenys una entitat
            
        }
        else if (estat == states.PLE){
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsObjecteCua[entitat] = tempsCua;
            objecteLlibreria.GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);


            if (cuaObjecte.Count != 0){
                int nDisponible = cercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
                    while (objectesRebutjats.Count != 0) {
                        // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                        if (AvisaDisponibilitat()) {
                            break;
                        }
                    }
                    estat = states.NOBUIT;
                } else {
                    estat = states.NOBUIT;
                }
            } else {
                while (objectesRebutjats.Count != 0) {
                    // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                    if (AvisaDisponibilitat()) {
                        break;
                    }
                }
                estat = states.BUIT;
            }
            return true; // En estat NOBUIT, si ens arriba una notificacioDisponibilitat sempre podrem enviar almenys una entitat
        }
        return false;
    }

    public void afegeixSeguentObjecte(GameObject objecte){
        if (!SeguentsObjectes.Contains(objecte)){
            GameObject objecteAmbLinia = new GameObject("L"+SeguentsObjectes.Count.ToString());
            objecteAmbLinia.transform.parent = transform;
            SeguentsObjectes.Add(objecte);
            LineRenderer lr = objecteAmbLinia.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, objecte.transform.position);
            lr.startColor = Color.green;
            lr.endColor = Color.green;
            lr.material = Resources.Load<Material>("Materials/LineRendererMaterial") as Material;

        }
    }

    // Retorna cert si l'objecte a qui s'avisa pot enviar-li una nova entitat
    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats.Dequeue();
        return objecteNou.GetComponent<IObjectes>().notificacioDisponible(this.gameObject);

    }


    public int getState(){
        return (int)estat;
    }

    public int ObteTipusObjecte()
    {
        return 1;
    }

    public void GenerarPlots(){
        // Per implementar
    }

    //////////////////////////////////////////////////////////////////////
    //                                                                  //
    //                                                                  //
    //                           FUNCIONS UI                            //
    //                                                                  //
    //                                                                  //
    //////////////////////////////////////////////////////////////////////
    public void OnMouseDown()
    {
        MotorSimuladorScript motorScript = gameObject.transform.parent.GetComponent<MotorSimuladorScript>();
        if (motorScript.AlgunDetallsObert())
        {
            motorScript.TancaDetallsObert();
        }
        if (UIScript.Instancia.obteBotoSeleccionat() == 6) motorScript.eliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 7)motorScript.ObreDetallsFill(transform.GetSiblingIndex());
        else if (UIScript.Instancia.obteBotoSeleccionat() == 4) UIScript.Instancia.ajuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 5) UIScript.Instancia.desjuntarObjectes(this.gameObject);
    }

    public void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UICuaScript>().CancelaCanvis();
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
