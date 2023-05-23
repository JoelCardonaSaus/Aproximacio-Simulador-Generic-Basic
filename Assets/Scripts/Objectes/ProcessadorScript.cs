using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProcessadorScript : MonoBehaviour, IObjectes, ITractarEsdeveniment
{
    public int maxEntitatsParalel = 1;
    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    [SerializeField]
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { CONSTANT, BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    [SerializeField]
    public distribucionsProbabilitat distribucio;
    [SerializeField]
    public double[] parametres;
    public ISeguentNumero distribuidor;
    [SerializeField]
    public List<GameObject> SeguentsObjectes = new List<GameObject>(); 
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void IniciaSimulacio(){
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

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) {
            Destroy(transform.GetChild(SeguentsObjectes.IndexOf(objecte)+1).gameObject);
            SeguentsObjectes.Remove(objecte);
        }
    }

    public void generarEsdevenimentProces(GameObject entitat, float tempsActual){
        Debug.Log("Es genera un esdeveniment per a un processador " + tempsActual.ToString());
        float tempsProcessat = (float)distribuidor.getNextSample();
        tempsMigEntitatsProcessador+=tempsProcessat;
        entitatsProcessant.Add(entitat);
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsProcessat, entitat, Esdeveniment.Tipus.PROCESSOS);
        transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
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
                    int nDisponible = cercaDisponible();
                    if (nDisponible != -1) {
                        ++nEntitatsEnviades;
                        SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
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
                        int nDisponible = cercaDisponible();
                        if (nDisponible != -1){
                            ++nEntitatsEnviades;
                            SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
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
        return objecteNou.GetComponent<IObjectes>().notificacioDisponible(this.gameObject);
    }

    public bool estaDisponible(GameObject objecteLlibreria)
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

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == estats.DISPONIBLE){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsDisponible += (tActual-ultimTemps);
            ultimTemps = tActual;
            generarEsdevenimentProces(entitat, tActual);
            if (maxEntitatsParalel == 1) estat = estats.BLOQUEJAT;
            else estat = estats.PROCESSANT;
        }
        else if (estat == estats.PROCESSANT){
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsProcessat += (tActual-ultimTemps);
            ultimTemps = tActual;
            generarEsdevenimentProces(entitat, tActual);
            if (maxEntitatsParalel != -1 && maxEntitatsParalel > entitatsProcessant.Count) estat = estats.PROCESSANT;
            else estat = estats.BLOQUEJAT;
        }
    }

    public bool notificacioDisponible(GameObject objecteLlibreria)
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
                objecteLlibreria.GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
                if (entitatsAEnviar.Count != 0) {
                    int nDisponible = cercaDisponible();
                    if (nDisponible != -1){
                        entitat = entitatsAEnviar.Dequeue();
                        SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
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

    public void desajuntarSeguentObjecte(GameObject desjuntar){
        intentaEliminarObjecteSeguents(desjuntar);
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

    public void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();

        float tempsActual = (transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());
        if (estat == estats.DISPONIBLE) tempsDisponible += (tempsActual - ultimTemps); 
        else if (estat == estats.PROCESSANT) tempsProcessat += (tempsActual - ultimTemps);
        else tempsBloquejat = (tempsActual - ultimTemps);
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
        if (UIScript.Instancia.obteBotoSeleccionat() == 6) motorScript.eliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 7)motorScript.ObreDetallsFill(transform.GetSiblingIndex());
        else if (UIScript.Instancia.obteBotoSeleccionat() == 4) UIScript.Instancia.ajuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 5) UIScript.Instancia.desjuntarObjectes(this.gameObject);

    }

    public void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UIProcessadorScript>().CancelaCanvis();
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
        return 2;
    }

    public void ActualitzaPropietats(politiquesEnrutament novaPolitica, distribucionsProbabilitat d, double[] nousParametres, int nEntitatsParalelMax){
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
