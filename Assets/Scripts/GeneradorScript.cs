using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorScript : MonoBehaviour, IRebreObjecte
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    private distribucionsProbabilitat currentDistribution = distribucionsProbabilitat.EXPONENTIAL;
    public double[] parametres = new double[1];
    public ISeguentNumero distribuidor;
    public List<GameObject> SeguentsObjectes;
    public GameObject entitatTemporal;
    private double timeForNextObject;
    private double timeScale;

    //Variables per als estadistics
    private int nEntitatsGenerades = 0;
    private List<double> tempsEntreEntitats = new List<double>();

    // UNA VEGADA COMENÇA LA SIMULACIÓ NO ES POT CANVIAR LA DISTRIBUCIÓ DE L'OBJECTE
    void Start()
    {
        switch (distribucio)
        {
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
        timeForNextObject = distribuidor.getNextSample()*60;
        timeScale = 1;
        //TODO: QUE EL MOTOR DE SIMULACIÓ SIGUI EL QUE INDIQUI LA ESCALA DEL TEMPS A CADA OBJECTE (GETSCALETIME)
    }

    // Update is called once per frame
    void Update()
    {
        if (timeForNextObject - (Time.deltaTime * timeScale) < 0){
            if (sendObject()){
                double rest = timeForNextObject - (Time.deltaTime * timeScale);
                timeForNextObject = (distribuidor.getNextSample()*60) + rest;
                ++nEntitatsGenerades;
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
                timeForNextObject = distribuidor.getNextSample() * 60;
                ++nEntitatsGenerades;
                tempsEntreEntitats.Add(timeForNextObject);
            }
        }
    }

    // Funcio per comprovar si es canvia la distribució del objecte
    private void OnValidate() {
        if (currentDistribution != distribucio){
            checkNumberOfParameters();    
            currentDistribution = distribucio;
        } 
    }

    private void checkNumberOfParameters(){

        if (distribucio == distribucionsProbabilitat.EXPONENTIAL || distribucio == distribucionsProbabilitat.POISSON){
            parametres = new double[1];
        }
        else if (distribucio == distribucionsProbabilitat.NORMAL){
            parametres = new double[2];
        }
        else {
            parametres = new double[3];
        }
    }

    public bool sendObject(){
        IRebreObjecte NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                foreach (GameObject objecte in SeguentsObjectes)
                {
                    NextObjecte = objecte.GetComponent<IRebreObjecte>();
                    if (NextObjecte.isAvailable()) {
                        GameObject newEntity = Instantiate(entitatTemporal, new Vector3(0,0,0), Quaternion.identity);
                        NextObjecte.recieveObject(newEntity);
                        return true;
                    }
                }
                return false;
            }

            else if (enrutament == politiquesEnrutament.RANDOM){
                bool[] attemts = new bool[SeguentsObjectes.Count]; // Teoricament, al crearse el valor per defecte es false
                int numIntents = 0;
                while (numIntents < attemts.Length){
                    int intent = Random.Range(0, attemts.Length - 1);
                    if (!attemts[intent]) {
                        ++numIntents;
                        attemts[intent] = true;
                        NextObjecte = SeguentsObjectes[intent].GetComponent<IRebreObjecte>();
                        if (NextObjecte.isAvailable()) {
                            GameObject newEntity = Instantiate(entitatTemporal, new Vector3(0,0,0), Quaternion.identity);
                            NextObjecte.recieveObject(newEntity);
                            return true;
                        }
                    }
                }
                
            }
        }
        return false;
    }

    private int nDisponibles(){
        int n = 0;
        foreach (GameObject seguent in SeguentsObjectes){
            if (seguent.GetComponent<IRebreObjecte>().isAvailable()) ++n;
        }
        return n;
    }

    public bool isAvailable(){
        return false;
    }
    public void recieveObject(GameObject entity){

    }
}