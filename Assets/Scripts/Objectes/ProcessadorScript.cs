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
    private Dictionary<GameObject, double> entitatsProcessant;
    //Variables per als estadistics
    private int nEntitatsTractades = 0;
    private int nEntitatsEntrades = 0;
    private double tempsMigEntitatsProcessador;
    private enum estats { ATURAT, PROCESSANT };
    private estats estat;
    private Queue<GameObject> entitatsAEnviar;
    private Queue<GameObject> objectesRebutjats;



    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void IniciaSimulacio(){
        estat = estats.ATURAT;
        nEntitatsEntrades = 0;
        nEntitatsTractades = 0;
        tempsMigEntitatsProcessador = 0;
        entitatsAEnviar = new Queue<GameObject>();
        objectesRebutjats = new Queue<GameObject>();
        entitatsProcessant = new Dictionary<GameObject, double>();
    }

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) SeguentsObjectes.Remove(objecte);
    }

    public void generarEsdevenimentProces(GameObject entitat, float tempsActual){
        Debug.Log("Es genera un esdeveniment per a un processador " + tempsActual.ToString());
        float tempsProcessat = (float)distribuidor.getNextSample();
        tempsMigEntitatsProcessador+=tempsProcessat;
        entitatsProcessant.Add(entitat, tempsActual);
        estat = estats.PROCESSANT;
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsProcessat, entitat, Esdeveniment.Tipus.PROCESSOS);
        transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        Debug.Log("Una entitat s'ha processat");
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.PROCESSOS:
                if (estat == estats.PROCESSANT){
                    ++nEntitatsTractades;
                    entitatsProcessant.Remove(e.ObteEntitatImplicada());
                    if (entitatsAEnviar.Count == 0){
                        int nDisponible = cercaDisponible();
                        if (nDisponible != -1){
                            SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(e.ObteEntitatImplicada(), this.gameObject);
                        } else {
                            entitatsAEnviar.Enqueue(e.ObteEntitatImplicada());
                        }
                    } else {
                        entitatsAEnviar.Enqueue(e.ObteEntitatImplicada());
                    }
                    if (entitatsAEnviar.Count == maxEntitatsParalel) while (objectesRebutjats.Count != 0 && !AvisaDisponibilitat());
                    if (entitatsProcessant.Count == 0) estat = estats.ATURAT;
                }
                else if (estat == estats.ATURAT){
                    // Si entra per aqui mostrar una exepcio
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
        if (maxEntitatsParalel == -1 || entitatsProcessant.Count < maxEntitatsParalel) return true;
        else {
            if (!objectesRebutjats.Contains(objecteLlibreria)) objectesRebutjats.Enqueue(objecteLlibreria);
        }
        return false;
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        ++nEntitatsEntrades;
        if (estat == estats.ATURAT){
            estat = estats.PROCESSANT;
        }
        float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
        generarEsdevenimentProces(entitat, tActual);
    }

    public bool notificacioDisponible(GameObject objecteLlibreria)
    {
        if (entitatsAEnviar.Count != 0){
            objecteLlibreria.GetComponent<IObjectes>().repEntitat(entitatsAEnviar.Dequeue(), this.gameObject);
            return true;
        }
        return false;
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
