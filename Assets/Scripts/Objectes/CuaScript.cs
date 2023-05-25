using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuaScript : LlibreriaObjectes
{
    
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    //public List<GameObject> SeguentsObjectes;
    //public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    //public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte;
    private Dictionary<GameObject, double> tempsObjecteCua;
    private Queue<GameObject> objectesRebutjats;
    private enum states { BUIT, NOBUIT, PLE };
    private states estat;

    private float tempsBuit = 0;
    private float tempsNoBuit = 0;
    private float tempsPle = 0;
    private float ultimTemps = 0;
    private int entitatsEnviades = 0;


    void Start()
    {
    }

    void Update()
    {
    }

    public override void IniciaSimulacio(){
        estat = states.BUIT;
        objectesRebutjats = new Queue<GameObject>();
        tempsObjecteCua = new Dictionary<GameObject, double>();
        cuaObjecte = new Queue<GameObject>();
        tempsBuit = 0;
        tempsNoBuit = 0;
        tempsPle = 0;
        ultimTemps = 0;
        entitatsEnviades = 0;
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == states.NOBUIT){
            cuaObjecte.Enqueue(entitat);
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsObjecteCua.Add(entitat, tActual);
            tempsNoBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            if (capacitatMaxima != -1 && cuaObjecte.Count < capacitatMaxima) estat = states.NOBUIT;
            else estat = states.PLE;
        }
        else if (estat == states.BUIT){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            int nDisponible = CercaDisponible();
            tempsBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            if (nDisponible != -1){
                SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                tempsObjecteCua.Add(entitat, 0);
                ++entitatsEnviades;
            } else {
                cuaObjecte.Enqueue(entitat);
                tempsObjecteCua.Add(entitat, tActual);
                if (capacitatMaxima!=-1 && capacitatMaxima > 1) estat = states.NOBUIT;
                else estat = states.PLE;
            }
        }
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
        if (estat == states.BUIT){
            tempsBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            return false;
        }
        else if (estat == states.NOBUIT){
            tempsNoBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsObjecteCua[entitat] = tempsCua;
            ++entitatsEnviades;
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);


            if (cuaObjecte.Count != 0){
                int nDisponible = CercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
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
            tempsPle += (tActual - ultimTemps);
            ultimTemps = tActual;
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsObjecteCua[entitat] = tempsCua;
            ++entitatsEnviades;
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);


            if (cuaObjecte.Count != 0){
                int nDisponible = CercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
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

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            if (estat == states.BUIT) tempsBuit += (tActual-ultimTemps);
            else if (estat == states.NOBUIT) tempsNoBuit += (tActual-ultimTemps);
            ultimTemps = tActual;
            return true;
        } 
        else{
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsPle += (tActual-ultimTemps);
            ultimTemps = tActual;
            if (!objectesRebutjats.Contains(objecteLlibreria))  objectesRebutjats.Enqueue(objecteLlibreria);
            return false;
        }
    }
    
    public override int ObteTipusObjecte()
    {
        return 1;
    }

    /*
    public override void IntentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) {
            Destroy(transform.GetChild(SeguentsObjectes.IndexOf(objecte)+1).gameObject);
            SeguentsObjectes.Remove(objecte);
        }
    }

    public override void AfegeixSeguentObjecte(GameObject objecte){
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
    
    public void desajuntarSeguentObjecte(GameObject desjuntar){
        intentaEliminarObjecteSeguents(desjuntar);
    }
    */

    


    /*
    public int CercaDisponible(){   
        LlibreriaObjectess SeguentObj;

        // Comprovem que almenys hi ha un objecte disponible
        if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
            for (int i = 0; i < SeguentsObjectes.Count; i++)//GameObject objecte in SeguentsObjectes)
            {
                SeguentObj = SeguentsObjectes[i].GetComponent<LlibreriaObjectess>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return i;
                }
            }
        }

        else if (enrutament == politiquesEnrutament.RANDOM){
            for (int i = 0; i < SeguentsObjectes.Count; i++){
                int obj = Random.Range(0, SeguentsObjectes.Count);
                SeguentObj = SeguentsObjectes[obj].GetComponent<LlibreriaObjectess>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return obj;
                }
            }
        }
        return -1;
    }
    */

    // Retorna cert si l'objecte a qui s'avisa pot enviar-li una nova entitat
    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats.Dequeue();
        return objecteNou.GetComponent<LlibreriaObjectes>().NotificacioDisponible(this.gameObject);

    }


    public int getState(){
        return (int)estat;
    }


    public override void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();

        float tempsActual = (transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());
        if (estat == states.BUIT) tempsBuit += (tempsActual - ultimTemps); 
        else if (estat == states.NOBUIT) tempsNoBuit += (tempsActual - ultimTemps);
        else tempsPle = (tempsActual - ultimTemps);
        double[] tempsEstats = new double[3] { tempsBuit, tempsNoBuit, tempsPle };
        string[] etiquetes = new string[3] { "Buit", "NoBuit", "Ple" };
        string nomImatge = "TempsEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(0, tempsEstats, etiquetes, "Temps", nomImatge);

        nomImatge = "PercentatgeEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(2, tempsEstats, etiquetes, "Percentatge", nomImatge);

        double[] nEntitatsEstadistic = new double[1] { entitatsEnviades };
        etiquetes = new string[1] { gameObject.transform.name };
        nomImatge = "Output"+gameObject.transform.name;
        eC.GeneraEstadistic(0, nEntitatsEstadistic, etiquetes, "Sortides",nomImatge);
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

    public override void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public override void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UICuaScript>().CancelaCanvis();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public override bool RatoliSobreDetalls(){
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
