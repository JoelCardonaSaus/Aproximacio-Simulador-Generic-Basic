using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneradorScript : MonoBehaviour, IObjectes, ITractarEsdeveniment
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { CONSTANT, BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres = new double[] {1,1,1}; // Inicialitza per evitar problemes
    public ISeguentNumero distribuidor;
    public List<GameObject> SeguentsObjectes;
    public GameObject entitatTemporal;
    private double tempsSeguentEntitat;
    public enum estats { GENERANT, BLOQUEJAT };
    public estats estat;

    //Variables per als estadistics
    private int nEntitatsGenerades = 0;
    private float tempsGenerant = 0;
    private float tempsBloquejat = 0;
    private float ultimTemps = 0;
    private List<double> tempsEntreEntitats = new List<double>();


    void Start()
    {
        distribuidor = new ConstantDistribution(5);
        estat = estats.GENERANT;
    }

    void Update()
    {   
        if (estat == estats.GENERANT){
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (estat == estats.BLOQUEJAT){
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void IniciaSimulacio(){
        estat = estats.GENERANT;
        nEntitatsGenerades = 0;
        tempsGenerant = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        generarEsdevenimentArribada(transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());
        tempsEntreEntitats = new List<double>();
    }

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) {
            Destroy(transform.GetChild(SeguentsObjectes.IndexOf(objecte)+1).gameObject);
            SeguentsObjectes.Remove(objecte);  
        }
    }

    public void afegeixSeguentObjecte(GameObject objecte){
        if (!SeguentsObjectes.Contains(objecte)){
            DibuixaLinia(objecte);
        }
    }

    public void desajuntarSeguentObjecte(GameObject desjuntar){
        intentaEliminarObjecteSeguents(desjuntar);
    }

    public void generarEsdevenimentArribada(float tempsActual){
        if (distribuidor==null) distribuidor = new ConstantDistribution(5);
        Debug.Log("Es genera un esdeveniment " + tempsActual.ToString());
        tempsSeguentEntitat = distribuidor.getNextSample();
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsSeguentEntitat, null, Esdeveniment.Tipus.ARRIBADES);
        transform.parent.GetComponent<MotorSimuladorScript>().afegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.ARRIBADES:
                if (estat == estats.GENERANT){
                    int objecteAEnviar = cercaDisponible();
                    tempsGenerant += (e.temps - ultimTemps);
                    ultimTemps = e.temps;
                    if (objecteAEnviar != -1) { // Si hi ha algun dels seguents objectes disponible, aleshores s'instancia una nova entitat temporal i s'envia l'entitat al objecte disponible
                        GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        SeguentsObjectes[objecteAEnviar].GetComponent<IObjectes>().repEntitat(novaEntitat, this.gameObject);
                        ++nEntitatsGenerades;
                        if (tempsEntreEntitats.Count != 0) {
                            tempsEntreEntitats.Add(e.temps-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
                        } else {
                            tempsEntreEntitats.Add(e.temps);
                        }
                        generarEsdevenimentArribada(e.temps); // Es programa la seguent arribada
                    } else { // Si no hi ha cap disponible, alehores el generador es bloqueja fins que algun objecte li demana una entitat
                        estat = estats.BLOQUEJAT;
                    }
                }
                else if (estat == estats.BLOQUEJAT){
                    // Si esta bloquejat espera que algú li demani un objecte
                    tempsBloquejat += (e.temps - ultimTemps);
                    ultimTemps = e.temps;
                }
                
                break;
        }
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

     // Per parametre es passa el gameobject de la llibreria que avisa de la seva disponibilitat
    public bool notificacioDisponible(GameObject objecteLlibreria){
        if (estat == estats.GENERANT) return false;
        else if (estat == estats.BLOQUEJAT)
        {
            estat = estats.GENERANT;
            ++nEntitatsGenerades;
            GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
            objecteLlibreria.GetComponent<IObjectes>().repEntitat(novaEntitat, this.gameObject);
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsBloquejat += (tActual - ultimTemps);
            ultimTemps = tActual;
            generarEsdevenimentArribada(tActual); // Es programa un nou esdeveniment d'arribada
            if (tempsEntreEntitats.Count != 0) {
                tempsEntreEntitats.Add(tActual-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
            } else {
                tempsEntreEntitats.Add(tActual);
            }
            return true;
        }
        return false;
    }

    public bool estaDisponible(GameObject objectePropietari){
        return false;
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria){} // El generador mai rebra una entitat

    public int getNGenerats(){
        return nEntitatsGenerades;
    }

    public int ObteTipusObjecte()
    {
        return 0;
    }

    public void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();
        double[] nEntitatsEstadistic = new double[1] { nEntitatsGenerades };
        string [] etiquetes = new string[1] { gameObject.transform.name };
        string nomImatge = "Output"+gameObject.transform.name;
        eC.GeneraEstadistic(0, nEntitatsEstadistic, etiquetes, "Sortides",nomImatge);
        float tempsActual = (transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());

        if (estat == estats.BLOQUEJAT) tempsBloquejat += (tempsActual - ultimTemps); 
        else tempsGenerant += (tempsActual - ultimTemps);
        double[] tempsEstats = new double[2] { tempsGenerant, tempsBloquejat };
        etiquetes = new string[2] { "Generant", "Bloquejat" };
        nomImatge = "TempsEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(0, tempsEstats, etiquetes, "Temps", nomImatge);

        nomImatge = "PercentatgeEstats"+gameObject.transform.name;
        eC.GeneraEstadistic(2, tempsEstats, etiquetes, "Percentatge", nomImatge);
    }

    //////////////////////////////////////////////////////////////////////
    //                                                                  //
    //                                                                  //
    //                           FUNCIONS UI                            //
    //                                                                  //
    //                                                                  //
    //////////////////////////////////////////////////////////////////////
    public void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UIGeneradorScript>().CancelaCanvis();
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
    
    public void DibuixaLinia(GameObject objecte){
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