using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProcessadorScript : LlibreriaObjectes, ITractarEsdeveniment
{
    public int maxEntitatsParalel = 1;
    public enum distribucionsProbabilitat { CONSTANT, BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres;
    public ISeguentNumero distribuidor;
    private List<GameObject> entitatsProcessant;
    //Variables per als estadistics
    private int nEntitatsEnviades = 0;
    private double tempsMigEntitatsProcessador;
    private enum estats { DISPONIBLE, PROCESSANT, BLOQUEJAT };
    private estats estat;
    private Queue<GameObject> entitatsAEnviar;
    private Queue<GameObject> objectesRebutjats;
    private float tempsDisponible = 0;
    private float tempsProcessat = 0;
    private float tempsBloquejat = 0;
    private float ultimTemps = 0;



    // Start is called before the first frame update
    void Start()
    {   
        transform.name = transform.name.Replace("Clone", transform.parent.GetComponent<MotorSimuladorScript>().ObteIdSeguentObjecte().ToString());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void IniciaSimulacio(){
        estat = estats.DISPONIBLE;
        nEntitatsEnviades = 0;
        tempsMigEntitatsProcessador = 0;
        entitatsAEnviar = new Queue<GameObject>();
        objectesRebutjats = new Queue<GameObject>();
        entitatsProcessant = new List<GameObject>();
        tempsDisponible = 0;
        tempsProcessat = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == estats.DISPONIBLE){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsDisponible += (tActual-ultimTemps);
            ultimTemps = tActual;
            GenerarEsdevenimentProces(entitat, tActual);
            if (maxEntitatsParalel == 1) estat = estats.BLOQUEJAT;
            else estat = estats.PROCESSANT;
        }
        else if (estat == estats.PROCESSANT){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            GenerarEsdevenimentProces(entitat, tActual);
            if (maxEntitatsParalel == -1 || (maxEntitatsParalel > entitatsProcessant.Count)) estat = estats.PROCESSANT;
            else estat = estats.BLOQUEJAT;
        }
    }

    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
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
            if (entitatsAEnviar.Count > 0){
                ++nEntitatsEnviades;
                GameObject entitat = entitatsAEnviar.Dequeue();
                objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                if (entitatsAEnviar.Count != 0) {
                    int nDisponible = CercaDisponible();
                    if (nDisponible != -1){
                        entitat = entitatsAEnviar.Dequeue();
                        SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                        while (objectesRebutjats.Count != 0) {
                            // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                            if (AvisaDisponibilitat()) {
                                break;
                            }
                        }

                        if (entitatsAEnviar.Count == 0){
                            if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                            else estat = estats.PROCESSANT;
                        } else {
                            estat = estats.BLOQUEJAT;
                        }
                    } else {
                        estat = estats.BLOQUEJAT;
                    }
                } else {
                    while (objectesRebutjats.Count != 0) {
                        // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                        if (AvisaDisponibilitat()) {
                            break;
                        }
                    }
                    if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                    else estat = estats.PROCESSANT;
                }
                return true;
            }
        }
        return false;
    }

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        if (estat != estats.BLOQUEJAT){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            if (estat == estats.DISPONIBLE) tempsDisponible += (tActual-ultimTemps);
            else if (estat == estats.PROCESSANT) tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            return true;  
        } 
        else {
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsBloquejat += (tActual-ultimTemps);
            ultimTemps = tActual;
            if (!objectesRebutjats.Contains(objecteLlibreria)) objectesRebutjats.Enqueue(objecteLlibreria);
        }
        return false;
    }

    public override int ObteTipusObjecte()
    {
        return 2;
    }


    public void GenerarEsdevenimentProces(GameObject entitat, float tempsActual){
        Debug.Log("Es genera un esdeveniment per a un processador " + tempsActual.ToString());
        float tempsProcessat = (float)distribuidor.ObteSeguentNumero();
        tempsMigEntitatsProcessador+=tempsProcessat;
        entitatsProcessant.Add(entitat);
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsProcessat, entitat, Esdeveniment.Tipus.PROCESSOS);
        transform.parent.GetComponent<MotorSimuladorScript>().AfegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        Debug.Log("Una entitat s'ha processat");
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.PROCESSOS:
                if (estat == estats.PROCESSANT){
                    tempsProcessat += (e.temps-ultimTemps);
                    ultimTemps = e.temps;
                    GameObject entitat = e.ObteEntitatImplicada();
                    entitatsProcessant.Remove(entitat);
                    int nDisponible = CercaDisponible();
                    if (nDisponible != -1) {
                        ++nEntitatsEnviades;
                        SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                        while (objectesRebutjats.Count != 0) {
                            // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                            if (AvisaDisponibilitat()) {
                                break;
                            }
                        }
                        if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                        else estat = estats.PROCESSANT;
                    } else {
                        entitatsAEnviar.Enqueue(entitat);
                        estat = estats.BLOQUEJAT;
                    }
                }
                
                else if (estat == estats.BLOQUEJAT){
                    tempsBloquejat += (e.temps-ultimTemps);
                    ultimTemps = e.temps;
                    GameObject entitat = e.ObteEntitatImplicada();
                    entitatsProcessant.Remove(entitat);
                    if (entitatsAEnviar.Count > 0){
                        entitatsAEnviar.Enqueue(entitat);
                        estat = estats.BLOQUEJAT;
                    } else {
                        int nDisponible = CercaDisponible();
                        if (nDisponible != -1){
                            ++nEntitatsEnviades;
                            SeguentsObjectes[nDisponible].GetComponent<LlibreriaObjectes>().RepEntitat(entitat, this.gameObject);
                            while (objectesRebutjats.Count != 0) {
                                // A la funcio AvisaDisponibilitat es fa un Dequeue del objectesRebutjats
                                if (AvisaDisponibilitat()) {
                                    break;
                                }
                            }

                            if (entitatsProcessant.Count == 0) estat = estats.DISPONIBLE;
                            else estat = estats.PROCESSANT;
                        } else {
                            entitatsAEnviar.Enqueue(entitat);
                            estat = estats.BLOQUEJAT;
                        }
                    }
                }
                break;            
        }
    }

    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats.Dequeue();
        return objecteNou.GetComponent<LlibreriaObjectes>().NotificacioDisponible(this.gameObject);
    }

    public override void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();

        float tempsActual = (transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());
        if (estat == estats.DISPONIBLE) tempsDisponible += (tempsActual - ultimTemps); 
        else if (estat == estats.PROCESSANT) tempsProcessat += (tempsActual - ultimTemps);
        else tempsBloquejat += (tempsActual - ultimTemps);
        double[] tempsEstats = new double[3] { tempsDisponible, tempsProcessat, tempsBloquejat };
        string[] etiquetes = new string[3] { "Disponible", "Processant", "Bloquejat" };
        string nomImatge = "TempsEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(0, tempsEstats, etiquetes, "Temps", nomImatge);

        nomImatge = "PercentatgeEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(2, tempsEstats, etiquetes, "Percentatge", nomImatge);

        double[] nEntitatsEstadistic = new double[1] { nEntitatsEnviades };
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
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) motorScript.EliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 7)motorScript.ObreDetallsFill(transform.GetSiblingIndex());
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);

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

    public void ActualitzaPropietats(politiquesEnrutament novaPolitica, distribucionsProbabilitat d, double[] nousParametres, int nEntitatsParalelMax, string nom){
        transform.name = nom;
        enrutament = novaPolitica;
        distribucio = d;
        parametres = nousParametres;
        maxEntitatsParalel = nEntitatsParalelMax;
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
