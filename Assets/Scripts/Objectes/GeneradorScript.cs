using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneradorScript : MonoBehaviour, IObjectes, ITractarEsdeveniment
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres;
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
        timeForNextObject = 2;
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)timeForNextObject, null, Esdeveniment.Tipus.ARRIBADES);
        GameObject motor = GameObject.Find("MotorDeSimulacio");
        motor.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        //if (state)
        // Fer un switch per estat i esdeveniment
        // programar la seguent arribada
        generarEsdevenimentArribada(e.temps);
    }

    // UNA VEGADA COMENÇA LA SIMULACIÓ NO ES POT CANVIAR LA DISTRIBUCIÓ DE L'OBJECTE
    void Start()
    {
        switch (distribucio)
        {
            case distribucionsProbabilitat.BINOMIAL:
                distribuidor = new BinomialDistribution(parametres[0], parametres[1]);
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
        timeForNextObject = distribuidor.getNextSample();
        //TODO: QUE EL MOTOR DE SIMULACIÓ SIGUI EL QUE INDIQUI LA ESCALA DEL TEMPS A CADA OBJECTE (GETSCALETIME)
    }

    // Update is called once per frame
    void Update()
    {
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
    private void OnValidate() {
        if (nParameters() != parametres.Length){
            checkNumberOfParameters();    
        } 
    }

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
    }

    private int nParameters(){
        if (distribucio == distribucionsProbabilitat.EXPONENTIAL || distribucio == distribucionsProbabilitat.POISSON){
            return 1;
        }
        else if (distribucio == distribucionsProbabilitat.NORMAL || distribucio == distribucionsProbabilitat.BINOMIAL || distribucio == distribucionsProbabilitat.DISCRETEUNIFORM){
            return 2;
        }
        else {
            return 3;
        }
    }

    private void checkNumberOfParameters(){
        if (distribucio == distribucionsProbabilitat.EXPONENTIAL || distribucio == distribucionsProbabilitat.POISSON){
            parametres = new double[1];
        }
        else if (distribucio == distribucionsProbabilitat.NORMAL || distribucio == distribucionsProbabilitat.BINOMIAL || distribucio == distribucionsProbabilitat.DISCRETEUNIFORM){
            parametres = new double[2];
        }
        else {
            parametres = new double[3];
        }
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
                        GameObject newEntity = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        NextObjecte.recieveObject(newEntity);
                        ++nEntitatsGenerades;
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
                        GameObject newEntity = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        NextObjecte.recieveObject(newEntity);
                        ++nEntitatsGenerades;
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

    public void CanviaEnrutament(politiquesEnrutament novaPolitica){
        enrutament = novaPolitica;
    }
    
}