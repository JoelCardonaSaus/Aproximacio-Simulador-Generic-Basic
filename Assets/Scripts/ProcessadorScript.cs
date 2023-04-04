using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ProcessadorScript : MonoBehaviour, IRebreObjecte
{
    public int maxEntitatsParalel = 1;
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
    public List<GameObject> SeguentsObjectes = new List<GameObject>(); 
    private Dictionary<GameObject, double> entitatsProcessant = new Dictionary<GameObject, double>();
    private double timeScale;

    //Variables per als estadistics
    private int nEntitatsTractades = 0;
    private int nEntitatsEntrades = 0;
    private double tempsMigEntitatsProcessador;
    private enum states { IDLE, BUSY };

    private states state = states.IDLE;

    private double timeIdle = 0;
    private double timeBusy = 0;

    private Queue<GameObject> entitatsAEnviar = new Queue<GameObject>();


    // Start is called before the first frame update
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
                distribuidor = new PoissonDistribution(7);
                break;
            case distribucionsProbabilitat.TRIANGULAR:
                distribuidor = new TriangularDistribution(parametres[0], parametres[1], parametres[2]);
                break;
            default:
                distribuidor = new ExponentialDistribution(parametres[0]);
                break;
        }
        timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (entitatsProcessant.Count > 0){
            List<GameObject> entitats = new List<GameObject>(entitatsProcessant.Keys);
            foreach(GameObject entitat in entitats)
            {
                entitatsProcessant[entitat] -= Time.deltaTime;
                if (entitatsProcessant[entitat] < 0){
                    entitatsAEnviar.Enqueue(entitat);
                    entitatsProcessant.Remove(entitat);
                    ++nEntitatsTractades;
                }
            }
            timeBusy += Time.deltaTime;
            if (entitatsProcessant.Count == 0) state = states.IDLE;
        }
        else {
            timeIdle += Time.deltaTime;
        }
        if (entitatsAEnviar.Count > 0){
            sendObject();
        }

    }

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

    public bool isAvailable()
    {
        if (entitatsProcessant.Count < maxEntitatsParalel) return true;
        else return false;
    }

    public void recieveObject(GameObject entity)
    {
        Debug.Log("El PROCESSADOR rep un objecte");
        entity.transform.position = transform.position + new Vector3(0,+1,0);
        if (state == states.IDLE) state = states.BUSY;
        ++nEntitatsEntrades;
        double tempsTractament = distribuidor.getNextSample();
        entitatsProcessant.Add(entity, tempsTractament);
        tempsMigEntitatsProcessador += tempsTractament;
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
                        NextObjecte.recieveObject(entitatsAEnviar.Dequeue());
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
                            NextObjecte.recieveObject(entitatsAEnviar.Dequeue());
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
}
