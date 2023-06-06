using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CuaScript : LlibreriaObjectes
{
    public TMP_Text etiquetes;
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    private Queue<GameObject> cuaObjecte = new Queue<GameObject>();
    private Dictionary<GameObject, double> tempsObjecteCua = new Dictionary<GameObject, double>();
    private Queue<GameObject> objectesRebutjats = new Queue<GameObject>();
    private enum states { BUIT, NOBUIT, PLE };
    private states estat;

    private float tempsBuit = 0;
    private float tempsNoBuit = 0;
    private float tempsPle = 0;
    private float ultimTemps = 0;
    private float tempsTotalEntitatsEnviades = 0;
    private int entitatsEnviades = 0;

    // Variable per poder moure els objectes
    Vector3 posicioRatoliOffset;

    void Start()
    {
        transform.name = transform.name.Replace("Clone", MotorSimuladorScript.Instancia.ObteIdSeguentObjecte().ToString());
        cuaObjecte = new Queue<GameObject>();
        string capacitat;
        if (capacitatMaxima == -1) capacitat = "∞";
        else capacitat = capacitatMaxima.ToString();
        etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
        GetComponent<SpriteRenderer>().material.shader = Shader.Find("GUI/Text Shader");
    }

    void Update()
    {
        if (estat == states.BUIT){
            GetComponent<SpriteRenderer>().color = Color.green;
            GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        else if (estat == states.NOBUIT){
            GetComponent<SpriteRenderer>().color = Color.yellow;
            GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }
        else {
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<SpriteRenderer>().material.color = Color.red;
        }
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
        tempsTotalEntitatsEnviades = 0;
        string capacitat;
        if (capacitatMaxima == -1) capacitat = "∞";
        else capacitat = capacitatMaxima.ToString();
        etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == states.NOBUIT){
            cuaObjecte.Enqueue(entitat);
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            tempsObjecteCua.Add(entitat, tActual);
            tempsNoBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            if (capacitatMaxima == -1 || capacitatMaxima > cuaObjecte.Count) estat = states.NOBUIT;
            else estat = states.PLE;
        }
        else if (estat == states.BUIT){
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            int nDisponible = CercaDisponible();
            tempsBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            if (nDisponible != -1){
                SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                ++entitatsEnviades;
            } else {
                cuaObjecte.Enqueue(entitat);
                tempsObjecteCua.Add(entitat, tActual);
                if (capacitatMaxima == -1 || capacitatMaxima > cuaObjecte.Count) estat = states.NOBUIT;
                else estat = states.PLE;
            }
        }
        string capacitat;
        if (capacitatMaxima == -1) capacitat = "∞";
        else capacitat = capacitatMaxima.ToString();
        etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
        if (estat == states.BUIT){
            tempsBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            return false;
        }
        else if (estat == states.NOBUIT){
            tempsNoBuit += (tActual - ultimTemps);
            ultimTemps = tActual;
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)MotorSimuladorScript.Instancia.ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsTotalEntitatsEnviades += tempsCua;
            tempsObjecteCua.Remove(entitat);
            ++entitatsEnviades;
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
           
            string capacitat;
            if (capacitatMaxima == -1) capacitat = "∞";
            else capacitat = capacitatMaxima.ToString();
            etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;

            if (cuaObjecte.Count != 0){
                int nDisponible = CercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)MotorSimuladorScript.Instancia.ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                    if (capacitatMaxima == -1) capacitat = "∞";
                    else capacitat = capacitatMaxima.ToString();
                    etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
                    estat = states.NOBUIT;
                } else {
                    estat = states.NOBUIT;
                }
            } else {
                estat = states.BUIT;
            }
            
            while (objectesRebutjats.Count != 0) {
                // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                if (AvisaDisponibilitat()) {
                    break;
                }
            }
            return true; // En estat NOBUIT, si ens arriba una notificacioDisponibilitat sempre podrem enviar almenys una entitat
            
        }
        else if (estat == states.PLE){
            tempsPle += (tActual - ultimTemps);
            ultimTemps = tActual;
            GameObject entitat = cuaObjecte.Dequeue();
            float tempsCua = (float)MotorSimuladorScript.Instancia.ObteTempsActual() - (float)tempsObjecteCua[entitat];
            tempsTotalEntitatsEnviades += tempsCua;
            tempsObjecteCua.Remove(entitat);
            ++entitatsEnviades;
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
            string capacitat;
            if (capacitatMaxima == -1) capacitat = "∞";
            else capacitat = capacitatMaxima.ToString();
            etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;

            if (cuaObjecte.Count != 0){
                int nDisponible = CercaDisponible();
                if (nDisponible != -1){
                    entitat = cuaObjecte.Dequeue();
                    tempsCua = (float)MotorSimuladorScript.Instancia.ObteTempsActual() - (float)tempsObjecteCua[entitat];
                    tempsObjecteCua[entitat] = tempsCua;
                    SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                    if (capacitatMaxima == -1) capacitat = "∞";
                    else capacitat = capacitatMaxima.ToString();
                    etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
                    estat = states.NOBUIT;
                } else {
                    estat = states.NOBUIT;
                }
            } else {
                estat = states.BUIT;
            }
            while (objectesRebutjats.Count != 0) {
                // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                if (AvisaDisponibilitat()) {
                    break;
                }
            }
            return true; // En estat NOBUIT, si ens arriba una notificacioDisponibilitat sempre podrem enviar almenys una entitat
        }
        return false;
    }

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima){
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            if (estat == states.BUIT) tempsBuit += (tActual-ultimTemps);
            else if (estat == states.NOBUIT) tempsNoBuit += (tActual-ultimTemps);
            ultimTemps = tActual;
            return true;
        } 
        else{
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            tempsPle += (tActual-ultimTemps);
            ultimTemps = tActual;
            if (!objectesRebutjats.Contains(objecteLlibreria))  objectesRebutjats.Enqueue(objecteLlibreria);
            return false;
        }
    }

    public override void ReiniciaSimulador(){
        estat = states.BUIT;
        objectesRebutjats = new Queue<GameObject>();
        tempsObjecteCua = new Dictionary<GameObject, double>();
        cuaObjecte = new Queue<GameObject>();
        tempsBuit = 0;
        tempsNoBuit = 0;
        tempsPle = 0;
        ultimTemps = 0;
        entitatsEnviades = 0;
        tempsTotalEntitatsEnviades = 0;
        string capacitat;
        if (capacitatMaxima == -1) capacitat = "∞";
        else capacitat = capacitatMaxima.ToString();
        etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
    }
    
    public override int ObteTipusObjecte()
    {
        return 1;
    }

    // Retorna cert si l'objecte a qui s'avisa pot enviar-li una nova entitat
    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats.Dequeue();
        return objecteNou.GetComponent<LlibreriaObjectes>().NotificacioDisponible(this.gameObject);

    }


    public int ObteEstat(){
        return (int)estat;
    }

    public int ObteEntitatsEnviades(){
        return entitatsEnviades;
    }


    public override void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();

        float tempsActual = (MotorSimuladorScript.Instancia.ObteTempsActual());
        if (estat == states.BUIT) tempsBuit += (tempsActual - ultimTemps); 
        else if (estat == states.NOBUIT) tempsNoBuit += (tempsActual - ultimTemps);
        else tempsPle += (tempsActual - ultimTemps);
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

    public override void ActualizaEstadistics(){
        string estadistics = "Output: " + entitatsEnviades+"\n";
        float tempsActual = (MotorSimuladorScript.Instancia.ObteTempsActual());
        etiquetes.text = cuaObjecte.Count.ToString();
        if (capacitatMaxima == -1) etiquetes.text += "/∞";
        else etiquetes.text += "/" + capacitatMaxima;
        etiquetes.text += "\n" + transform.name + "\n";

        if (estat == states.BUIT){
            tempsBuit += (tempsActual - ultimTemps); 
        } 
        else if (estat == states.NOBUIT){
            tempsNoBuit += (tempsActual - ultimTemps);
        }
        else {
            tempsPle += (tempsActual - ultimTemps);
        }
        ultimTemps = tempsActual;
        float percBuit = 100*(tempsBuit/(tempsActual));
        float percNoBuit = 100*(tempsNoBuit/(tempsActual));
        float percPle = 100*(tempsPle/(tempsActual));
        if (entitatsEnviades != 0) estadistics += "Temps mig entitats a la cua: " + (tempsTotalEntitatsEnviades/entitatsEnviades) +"\n";
        estadistics += "% Buit: " + percBuit + "\n";
        estadistics += "% No Buit: " + percNoBuit + "\n";
        estadistics += "% Ple: " + percPle + "\n";

        
        etiquetes.text += estadistics; 
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
        posicioRatoliOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MotorSimuladorScript.Instancia.AlgunDetallsObert())
        {
            MotorSimuladorScript.Instancia.TancaDetallsObert();
        }
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) MotorSimuladorScript.Instancia.EliminarObjecte(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 7)MotorSimuladorScript.Instancia.ObreDetallsFill(transform.GetSiblingIndex());
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);
    }

    private void OnMouseDrag(){
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + posicioRatoliOffset;
        for (int i = 0; i < SeguentsObjectes.Count; i++){
            LineRenderer lr = gameObject.transform.GetChild(2+i).GetComponent<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, SeguentsObjectes[i].transform.position);
        }
        for (int i = 0; i < ObjectesPredecessors.Count; i++){
            ObjectesPredecessors[i].GetComponent<LlibreriaObjectes>().CanviaPosicioPredecessor(this.gameObject);
        }
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

    public void ActualitzaPropietats(politiquesEnrutament nouEnrutament, int capacitatMax, string nom){
        transform.name = nom;
        enrutament = nouEnrutament;
        capacitatMaxima = capacitatMax;
        string capacitat;
        if (capacitatMaxima == -1) capacitat = "∞";
        else capacitat = capacitatMaxima.ToString();
        etiquetes.text = cuaObjecte.Count+"/"+capacitat+"\n"+transform.name;
    }
}
