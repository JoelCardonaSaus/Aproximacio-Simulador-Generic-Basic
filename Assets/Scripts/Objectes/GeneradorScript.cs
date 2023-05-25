using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneradorScript : LlibreriaObjectes, ITractarEsdeveniment
{

    public enum distribucionsProbabilitat { CONSTANT, BINOMIAL, DISCRETEUNIFORM, EXPONENTIAL, NORMAL, POISSON, TRIANGULAR };
    public distribucionsProbabilitat distribucio;
    public double[] parametres = new double[] {1,1,1}; // Inicialitza per evitar problemes
    public ISeguentNumero distribuidor;
    public GameObject[] entitatsTemporals = new GameObject[3];
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

    public override void IniciaSimulacio(){
        estat = estats.GENERANT;
        nEntitatsGenerades = 0;
        tempsGenerant = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        GenerarEsdevenimentArribada(transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual());
        tempsEntreEntitats = new List<double>();
        entitatTemporal = entitatsTemporals[transform.parent.GetComponent<MotorSimuladorScript>().ObteEntitatsSeleccionades()];
    }
    
    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria){} // El generador mai rebra una entitat

    // Per parametre es passa el gameobject de la llibreria que avisa de la seva disponibilitat
    public override bool NotificacioDisponible(GameObject objecteLlibreria){
        if (estat == estats.GENERANT) return false;
        else if (estat == estats.BLOQUEJAT)
        {
            estat = estats.GENERANT;
            ++nEntitatsGenerades;
            GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(novaEntitat, this.gameObject);
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
                    Debug.Log("TempsActual + ultim " + tActual + " " + ultimTemps);
            tempsBloquejat += (tActual - ultimTemps);
            ultimTemps = tActual;
            GenerarEsdevenimentArribada(tActual); // Es programa un nou esdeveniment d'arribada
            if (tempsEntreEntitats.Count != 0) {
                tempsEntreEntitats.Add(tActual-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
            } else {
                tempsEntreEntitats.Add(tActual);
            }
            return true;
        }
        return false;
    }

    public override bool EstaDisponible(GameObject objectePropietari){
        return false;
    }
    
    public void GenerarEsdevenimentArribada(float tempsActual){
        if (distribuidor==null) distribuidor = new ConstantDistribution(5);
        Debug.Log("Es genera un esdeveniment " + tempsActual.ToString());
        tempsSeguentEntitat = distribuidor.ObteSeguentNumero();
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsSeguentEntitat, null, Esdeveniment.Tipus.ARRIBADES);
        transform.parent.GetComponent<MotorSimuladorScript>().AfegirEsdeveniment(e);
    }

    public void TractarEsdeveniment(Esdeveniment e){
        switch (e.tipusEsdeveniment)
        {
            case Esdeveniment.Tipus.ARRIBADES:
                if (estat == estats.GENERANT){
                    int objecteAEnviar = CercaDisponible();
                    tempsGenerant += (e.temps - ultimTemps);
                    ultimTemps = e.temps;
                    if (objecteAEnviar != -1) { // Si hi ha algun dels seguents objectes disponible, aleshores s'instancia una nova entitat temporal i s'envia l'entitat al objecte disponible
                        GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);
                        SeguentsObjectes[objecteAEnviar].GetComponent<LlibreriaObjectes>().RepEntitat(novaEntitat, this.gameObject);
                        ++nEntitatsGenerades;
                        if (tempsEntreEntitats.Count != 0) {
                            tempsEntreEntitats.Add(e.temps-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
                        } else {
                            tempsEntreEntitats.Add(e.temps);
                        }
                        GenerarEsdevenimentArribada(e.temps); // Es programa la seguent arribada
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

    public override int ObteTipusObjecte()
    {
        return 0;
    }


    public int getNGenerats(){
        return nEntitatsGenerades;
    }


    public override void GenerarPlots(){
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
    public override void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public override void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UIGeneradorScript>().CancelaCanvis();
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
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) motorScript.EliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 7){
            motorScript.ObreDetallsFill(transform.GetSiblingIndex());
        }
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);
    }

}