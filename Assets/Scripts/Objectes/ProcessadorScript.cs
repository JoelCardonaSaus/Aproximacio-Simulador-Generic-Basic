using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProcessadorScript : LlibreriaObjectes, ITractarEsdeveniment
{
    public TMP_Text etiquetes;
    public int maxEntitatsParalel = -1;
    public enum distribucionsProbabilitat { CONSTANT, BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres;
    public ISeguentNumero distribuidor;
    private List<GameObject> entitatsProcessant = new List<GameObject>();
    //Variables per als estadistics
    private int nEntitatsEnviades = 0;
    private int nEntitatsTractades = 0;
    private double tempsMigEntitatsProcessador;
    private enum estats { DISPONIBLE, PROCESSANT, BLOQUEJAT };
    private estats estat;
    private Queue<GameObject> entitatsAEnviar = new Queue<GameObject>();
    private List<GameObject> objectesRebutjats = new List<GameObject>();
    private float tempsDisponible = 0;
    private float tempsProcessat = 0;
    private float tempsBloquejat = 0;
    private float ultimTemps = 0;
    // Variable per poder moure els objectes
    Vector3 posicioRatoliOffset;


    // Start is called before the first frame update
    void Start()
    {   
        transform.name = transform.name.Replace("Clone", MotorSimuladorScript.Instancia.ObteIdSeguentObjecte().ToString());
        entitatsProcessant = new List<GameObject>();
        entitatsAEnviar = new Queue<GameObject>();
        string capacitat;
        if (maxEntitatsParalel == -1) capacitat = "∞";
        else capacitat = maxEntitatsParalel.ToString();
        etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
        GetComponent<SpriteRenderer>().material.shader = Shader.Find("GUI/Text Shader");

    }

    // Update is called once per frame
    void Update()
    {
        if (estat == estats.DISPONIBLE){
            GetComponent<SpriteRenderer>().color = Color.green;
            GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        else if (estat == estats.PROCESSANT){
            GetComponent<SpriteRenderer>().color = Color.yellow;
            GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }
        else {
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<SpriteRenderer>().material.color = Color.red;
        }
    }

    public override void IniciaSimulacio(){
        estat = estats.DISPONIBLE;
        nEntitatsEnviades = 0;
        tempsMigEntitatsProcessador = 0;
        entitatsAEnviar = new Queue<GameObject>();
        objectesRebutjats = new List<GameObject>();
        entitatsProcessant = new List<GameObject>();
        tempsDisponible = 0;
        tempsProcessat = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        nEntitatsTractades = 0;
        string capacitat;
        if (maxEntitatsParalel == -1) capacitat = "∞";
        else capacitat = maxEntitatsParalel.ToString();
        etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
        Esdeveniment e = new Esdeveniment(objecteLlibreria, this.gameObject, tActual, entitat, Esdeveniment.Tipus.eRepEntitat);
        UIScript.Instancia.UltimEsdeveniment(e);
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == estats.DISPONIBLE){
            entitat.transform.SetParent(this.gameObject.transform);
            tempsDisponible += (tActual-ultimTemps);
            ultimTemps = tActual;
            GenerarEsdevenimentProces(entitat, tActual);
            estat = estats.PROCESSANT;
            string capacitat;
            if (maxEntitatsParalel == -1) capacitat = "∞";
            else capacitat = maxEntitatsParalel.ToString();
            etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
        }
        else if (estat == estats.PROCESSANT){
            entitat.transform.SetParent(this.gameObject.transform);
            tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            GenerarEsdevenimentProces(entitat, tActual);
            estat = estats.PROCESSANT;
            string capacitat;
            if (maxEntitatsParalel == -1) capacitat = "∞";
            else capacitat = maxEntitatsParalel.ToString();
            etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
            
        }
    }

    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
        Esdeveniment e = new Esdeveniment(objecteLlibreria, this.gameObject, tActual, null, Esdeveniment.Tipus.eNotificacioDisponible);
        UIScript.Instancia.UltimEsdeveniment(e);

        if (estat == estats.DISPONIBLE){
            tempsDisponible += (tActual-ultimTemps);
            ultimTemps = tActual;
            return false;
        }
        else if (estat == estats.PROCESSANT){
            tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            return false;
        }
        else if (estat == estats.BLOQUEJAT){
            tempsBloquejat += (tActual-ultimTemps);
            ultimTemps = tActual;
            ++nEntitatsEnviades;
            GameObject entitat = entitatsAEnviar.Dequeue();
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
            string capacitat;
            if (maxEntitatsParalel == -1) capacitat = "∞";
            else capacitat = maxEntitatsParalel.ToString();
            etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;

            bool rebutjat = false;
            while (entitatsAEnviar.Count != 0 && !rebutjat) {
                int nDisponible = CercaDisponible();
                if (nDisponible != -1){
                    entitat = entitatsAEnviar.Dequeue();
                    SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                    if (maxEntitatsParalel == -1) capacitat = "∞";
                    else capacitat = maxEntitatsParalel.ToString();
                    etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
                } else {
                    rebutjat = true;
                    estat = estats.BLOQUEJAT;
                }
            } 
            if (entitatsAEnviar.Count == 0){
                if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                else estat = estats.PROCESSANT;
                while (objectesRebutjats.Count != 0) {
                    // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                    if (AvisaDisponibilitat()) {
                        break;
                    }
                }
            }
            else estat = estats.BLOQUEJAT;
            return true;
            
        }
        return false;
    }

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        if (estat != estats.BLOQUEJAT && (maxEntitatsParalel > entitatsProcessant.Count || maxEntitatsParalel == -1)){
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            if (estat == estats.DISPONIBLE) tempsDisponible += (tActual-ultimTemps);
            else if (estat == estats.PROCESSANT) tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            return true;  
        } 
        else {
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            if (estat == estats.BLOQUEJAT) tempsBloquejat += (tActual-ultimTemps);
            else if (estat == estats.PROCESSANT) tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            objectesRebutjats.Add(objecteLlibreria);
        }
        return false;
    }

    public override void ReiniciaSimulador(){
        estat = estats.DISPONIBLE;
        nEntitatsEnviades = 0;
        tempsMigEntitatsProcessador = 0;
        entitatsAEnviar = new Queue<GameObject>();
        objectesRebutjats = new List<GameObject>();
        entitatsProcessant = new List<GameObject>();
        tempsDisponible = 0;
        tempsProcessat = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        nEntitatsTractades = 0;
        string capacitat;
        if (maxEntitatsParalel == -1) capacitat = "∞";
        else capacitat = maxEntitatsParalel.ToString();
        etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
    }

    public override int ObteTipusObjecte()
    {
        return 2;
    }


    public void GenerarEsdevenimentProces(GameObject entitat, float tempsActual){
        if (distribuidor==null) distribuidor = new ConstantDistribution(5);
        float tempsProcessat = (float)distribuidor.ObteSeguentNumero();
        tempsMigEntitatsProcessador+=tempsProcessat;
        entitatsProcessant.Add(entitat);
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+tempsProcessat, entitat, Esdeveniment.Tipus.PROCESSOS);
        MotorSimuladorScript.Instancia.AfegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.PROCESSOS:
                if (estat == estats.PROCESSANT){
                    tempsProcessat += (e.temps-ultimTemps);
                    ultimTemps = e.temps;
                    GameObject entitat = e.ObteEntitatImplicada();
                    ++nEntitatsTractades;
                    entitatsProcessant.Remove(entitat);
                    int nDisponible = CercaDisponible();
                    if (nDisponible != -1) {
                        ++nEntitatsEnviades;
                        SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                        if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                        else estat = estats.PROCESSANT;
                        while (objectesRebutjats.Count != 0) {
                            // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                            if (AvisaDisponibilitat()) {
                                break;
                            }
                        }
                    } else {
                        entitatsAEnviar.Enqueue(entitat);
                        estat = estats.BLOQUEJAT;
                    }
                    string capacitat;
                    if (maxEntitatsParalel == -1) capacitat = "∞";
                    else capacitat = maxEntitatsParalel.ToString();
                    etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
                }
                
                else if (estat == estats.BLOQUEJAT){
                    tempsBloquejat += (e.temps-ultimTemps);
                    ultimTemps = e.temps;
                    GameObject entitat = e.ObteEntitatImplicada();
                    ++nEntitatsTractades;
                    entitatsProcessant.Remove(entitat);
                    entitatsAEnviar.Enqueue(entitat);
                    estat = estats.BLOQUEJAT;
                    string capacitat;
                    if (maxEntitatsParalel == -1) capacitat = "∞";
                    else capacitat = maxEntitatsParalel.ToString();
                    etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
                }
                break;            
        }
    }

    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats[0];
        objectesRebutjats.RemoveAt(0);
        return objecteNou.GetComponent<LlibreriaObjectes>().NotificacioDisponible(this.gameObject);
    }
    
    public override void ActualizaEstadistics(){
        string estadistics = "Output: " + nEntitatsEnviades +"\n";
        float tempsActual = (MotorSimuladorScript.Instancia.ObteTempsActual());
        etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count).ToString();
        if (maxEntitatsParalel == -1) etiquetes.text += "/∞";
        else etiquetes.text += "/" + maxEntitatsParalel;
        etiquetes.text += "\n" + transform.name + "\n";

        if (estat == estats.DISPONIBLE){
            tempsDisponible += (tempsActual - ultimTemps); 
        } 
        else if (estat == estats.PROCESSANT){
            tempsProcessat += (tempsActual - ultimTemps);
        }
        else {
            tempsBloquejat += (tempsActual - ultimTemps);
        }
        ultimTemps = tempsActual;
        float percDisponible = (float)Math.Round(100*(tempsDisponible/(tempsActual)),2);
        float percProcessant = (float)Math.Round(100*(tempsProcessat/(tempsActual)),2);
        float percBloquejat = (float)Math.Round(100-(percDisponible+percProcessant),2);
        if (nEntitatsEnviades != 0) estadistics += "Temps mig processat: " + (float)Math.Round((tempsMigEntitatsProcessador/(entitatsProcessant.Count + nEntitatsTractades)),2) +"\n";
        estadistics += "% Disponible: " + percDisponible + "\n";
        estadistics += "% Processant: " + percProcessant + "\n";
        estadistics += "% Bloquejat: " + percBloquejat + "\n";

        
        etiquetes.text += estadistics; 

    }

    public int ObteEstat(){
        return (int)estat;
    }

    public int ObteEntitatsEnviades(){
        return nEntitatsEnviades;
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
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UIProcessadorScript>().CancelaCanvis();
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

    public void ActualitzaPropietats(politiquesEnrutament novaPolitica, distribucionsProbabilitat d, float[] nousParametres, int nEntitatsParalelMax, string nom){
        transform.name = nom;
        enrutament = novaPolitica;
        distribucio = d;
        double[] aux = new double[nousParametres.Length];
        for (int i = 0; i < nousParametres.Length; ++i) aux[i] = (double)nousParametres[i];
        parametres = aux;
        maxEntitatsParalel = nEntitatsParalelMax;
        string capacitat;
        if (maxEntitatsParalel == -1) capacitat = "∞";
        else capacitat = maxEntitatsParalel.ToString();
        etiquetes.text = (entitatsProcessant.Count+entitatsAEnviar.Count)+"/"+capacitat+"\n"+transform.name;
        ActualitzaDistribuidor();
    }

    public void ActualitzaDistribuidor(){
        switch (distribucio)
        {
            case distribucionsProbabilitat.BINOMIAL:
                distribuidor = new BinomialDistribution(parametres[0], parametres[1]);
                break;
            case distribucionsProbabilitat.CONSTANT:
                distribuidor = new ConstantDistribution(parametres[0]);
                break;
            case distribucionsProbabilitat.DISCRETEUNIFORM:
                distribuidor = new DiscreteUniformDistribution(parametres[0], parametres[1]);
                break;
            case distribucionsProbabilitat.EXPONENTIAL:
                distribuidor = new ExponentialDistribution(parametres[0]);
                break;
            case distribucionsProbabilitat.NORMAL:
                distribuidor = new NormalDistribution(parametres[0], parametres[1]);
                break;
            case distribucionsProbabilitat.POISSON:
                distribuidor = new PoissonDistribution(parametres[0]);
                break;
            case distribucionsProbabilitat.TRIANGULAR:
                distribuidor = new TriangularDistribution(parametres[0], parametres[1], parametres[2]);
                break;
            default:
                distribuidor = new ExponentialDistribution(parametres[0]);
                break;
        }
    }
}
