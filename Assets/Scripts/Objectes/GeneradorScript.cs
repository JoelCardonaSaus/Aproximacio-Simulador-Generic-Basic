using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneradorScript : MonoBehaviour, IObjectes, ITractarEsdeveniment
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { BINOMIAL, CONSTANT, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres = new double[] {1,1,1}; // Inicialitza per evitar problemes
    public ISeguentNumero distribuidor;
    public List<GameObject> SeguentsObjectes;
    public GameObject entitatTemporal;
    private double timeForNextObject;
    private float timeScale = 1;

    //Variables per als estadistics
    private int nEntitatsGenerades = 0;
    private List<double> tempsEntreEntitats = new List<double>();

    public void IniciaSimulacio(){
        nEntitatsGenerades = 0;
        generarEsdevenimentArribada(0);
    }

    public void generarEsdevenimentArribada(float tempsActual){
        Debug.Log("Es genera un esdeveniment " + tempsActual.ToString());
        timeForNextObject = distribuidor.getNextSample();
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)timeForNextObject, null, Esdeveniment.Tipus.ARRIBADES);
        transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.ARRIBADES:
                int objecteAEnviar = sendObject();
                if (objecteAEnviar != -1) { // Si hi ha algun dels seguents objectes disponible, aleshores s'instancia una nova entitat temporal i es crea un nou esdeveniment
                    GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                    Esdeveniment eNou = new Esdeveniment(this.gameObject, SeguentsObjectes[objecteAEnviar], e.temps+1, novaEntitat, Esdeveniment.Tipus.ARRIBADES);
                    transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(eNou);
                    if (tempsEntreEntitats.Count != 0) {
                        tempsEntreEntitats.Add(e.temps+tempsEntreEntitats[tempsEntreEntitats.Count-1]);
                    } else {
                        tempsEntreEntitats.Add(e.temps);
                    }
                    generarEsdevenimentArribada(e.temps);
                } else { // Si no hi ha cap disponible, alehores es programa un nou esdeveniment per dintre d'un segon més tard a veure si hi ha algún objecte disponible
                    Esdeveniment eNou = new Esdeveniment(this.gameObject, this.gameObject, e.temps+1, null, Esdeveniment.Tipus.ARRIBADES);
                    transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(eNou);
                }
                break;
            // De moment crec que no hauria de rebre esdeveniments d'aquest tipus el generador
            case Esdeveniment.Tipus.PROCESSOS:
                break;
        }
    }

    void Start()
    {
        distribuidor = new ConstantDistribution(5);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (timeForNextObject - (Time.deltaTime * timeScale) < 0){
            if (sendObject()){
                double rest = timeForNextObject - (Time.deltaTime * timeScale);
                timeForNextObject = (distribuidor.getNextSample()) + rest;
                tempsEntreEntitats.Add(timeForNextObject);
            } else {
                timeForNextObject = 0;
            }
        }
        else if (timeForNextObject - (Time.deltaTime * timeScale) > 0) {
            timeForNextObject -=  (Time.deltaTime * timeScale);
        }
        else {
            if(sendObject()){
                timeForNextObject = distribuidor.getNextSample();
                tempsEntreEntitats.Add(timeForNextObject);
            }
        }
        */
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

    // Funcio per comprovar si es canvia la distribució del objecte

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
    }

    // Retorna la posició dels seguents objectes. -1 si no hi ha cap, [0..n-1] si la llista de seguents objectes no es buida.
    public int sendObject(){
        IObjectes NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                for (int i = 0; i < SeguentsObjectes.Count; i++)//GameObject objecte in SeguentsObjectes)
                {
                    NextObjecte = SeguentsObjectes[i].GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable()) {
                        GameObject newEntity = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        ++nEntitatsGenerades;
                        return i;
                    }
                }
            }

            else if (enrutament == politiquesEnrutament.RANDOM){
                for (int i = 0; i < SeguentsObjectes.Count; i++){
                    int obj = Random.Range(0, SeguentsObjectes.Count);
                    NextObjecte = SeguentsObjectes[obj].GetComponent<IObjectes>();
                    if (NextObjecte.isAvailable()) {
                        GameObject newEntity = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        ++nEntitatsGenerades;
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
            if (seguent.GetComponent<IObjectes>().isAvailable()) ++n;
        }
        return n;
    }

    public bool isAvailable(){
        return false;
    }
    
    public void recieveObject(GameObject entity){

    }

    public int getNGenerats(){
        return nEntitatsGenerades;
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
        return 0;
    }

    public void ActualitzaPropietats(politiquesEnrutament novaPolitica, distribucionsProbabilitat d, double[] nousParametres){
        enrutament = novaPolitica;
        distribucio = d;
        parametres = nousParametres;
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