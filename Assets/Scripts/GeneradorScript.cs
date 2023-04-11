using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneradorScript : MonoBehaviour, IRebreObjecte
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    [SerializeField]
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    [SerializeField]
    public distribucionsProbabilitat distribucio;
    [SerializeField]
    public double[] parametres;
    public ISeguentNumero distribuidor;
    [SerializeField]
    public List<GameObject> SeguentsObjectes;
    public GameObject entitatTemporal;
    private double timeForNextObject;
    private float timeScale = 1;

    //Variables per als estadistics
    private int nEntitatsGenerades = 0;
    private List<double> tempsEntreEntitats = new List<double>();

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

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
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

    // Funcio per comprovar si es canvia la distribució del objecte
    private void OnValidate() {
        if (nParameters() != parametres.Length){
            checkNumberOfParameters();    
        } 
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
        IRebreObjecte NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                foreach (GameObject objecte in SeguentsObjectes)
                {
                    NextObjecte = objecte.GetComponent<IRebreObjecte>();
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
                    NextObjecte = SeguentsObjectes[obj].GetComponent<IRebreObjecte>();
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
            if (seguent.GetComponent<IRebreObjecte>().isAvailable()) ++n;
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
}