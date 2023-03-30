using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorScript : MonoBehaviour
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    private distribucionsProbabilitat currentDistribution = distribucionsProbabilitat.EXPONENTIAL;
    public double[] parametres;
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
        timeForNextObject = distribuidor.getNextSample();
        timeScale = 1;
        //TODO: QUE EL MOTOR DE SIMULACIÓ SIGUI EL QUE INDIQUI LA ESCALA DEL TEMPS A CADA OBJECTE (GETSCALETIME)
    }

    // Update is called once per frame
    void Update()
    {
        if (timeForNextObject - (Time.deltaTime * timeScale) < 0){
            if (sendObject()){
                double rest = timeForNextObject - (Time.deltaTime * timeScale);
                timeForNextObject = distribuidor.getNextSample() + rest;
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
                timeForNextObject = distribuidor.getNextSample();
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

    //POTSER AQUESTES DUES FUNCIONS VAN A NIVELL DE UN SCRIPT QUE TOTS HAN DE TENIR
    private bool sendObject(){
        if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
            /*
            foreach (var object in SeguentsObjectes)
            {
                // TODO: FER CRIDES
                if (objecte);
            }
            */
        }
        return false;
    }

    public bool recieveObject(GameObject entity){
        return false;
    }
}