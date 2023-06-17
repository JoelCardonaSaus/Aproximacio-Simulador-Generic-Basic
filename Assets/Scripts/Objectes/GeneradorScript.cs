using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneradorScript : LlibreriaObjectes, ITractarEsdeveniment
{
    public TMP_Text etiquetes;
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

    // Variable per poder moure els objectes
    Vector3 posicioRatoliOffset;


    void Start()
    {
        distribuidor = new ConstantDistribution(5);
        estat = estats.GENERANT;
        transform.name = transform.name.Replace("Clone", MotorSimuladorScript.Instancia.ObteIdSeguentObjecte().ToString());
        etiquetes.text = "0/1\n"+transform.name;
        GetComponent<SpriteRenderer>().material.shader = Shader.Find("GUI/Text Shader");
    }

    void Update()
    {   
        if (estat == estats.GENERANT){
            GetComponent<SpriteRenderer>().color = Color.green;
            GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        if (estat == estats.BLOQUEJAT){
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<SpriteRenderer>().material.color = Color.red;
        }
    }

    public override void IniciaSimulacio(){
        estat = estats.GENERANT;
        nEntitatsGenerades = 0;
        tempsGenerant = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        etiquetes.text = "0/1\n"+transform.name;
        GenerarEsdevenimentArribada(MotorSimuladorScript.Instancia.ObteTempsActual());
        tempsEntreEntitats = new List<double>();
    }
    
    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria){} // El generador mai rebra una entitat

    // Per parametre es passa el gameobject de la llibreria que avisa de la seva disponibilitat
    public override bool NotificacioDisponible(GameObject objecteLlibreria){
        if (estat == estats.GENERANT){
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            Esdeveniment e = new Esdeveniment(objecteLlibreria, this.gameObject, tActual, null, Esdeveniment.Tipus.eNotificacioDisponible);
            UIScript.Instancia.UltimEsdeveniment(e);
            return false;
        } 
        else if (estat == estats.BLOQUEJAT)
        {
            float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
            etiquetes.text = "0/1\n"+transform.name;
            estat = estats.GENERANT;
            ++nEntitatsGenerades;
            GameObject novaEntitat = Instantiate(entitatTemporal, transform.position + new Vector3(0,+1,0), Quaternion.identity);

            Esdeveniment e = new Esdeveniment(objecteLlibreria, this.gameObject, tActual, novaEntitat, Esdeveniment.Tipus.eNotificacioDisponible);
            UIScript.Instancia.UltimEsdeveniment(e);

            objecteLlibreria.GetComponent<LlibreriaObjectes>().RepEntitat(novaEntitat, this.gameObject);
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

    public override void ReiniciaSimulador(){
        estat = estats.GENERANT;
        nEntitatsGenerades = 0;
        tempsGenerant = 0;
        tempsBloquejat = 0;
        ultimTemps = 0;
        etiquetes.text = "0/1\n"+transform.name;
        tempsEntreEntitats = new List<double>();
    }
    
    public void GenerarEsdevenimentArribada(float tempsActual){
        if (distribuidor==null) distribuidor = new ConstantDistribution(5);
        tempsSeguentEntitat = distribuidor.ObteSeguentNumero();
        Esdeveniment e = new Esdeveniment(this.gameObject, this.gameObject, tempsActual+(float)tempsSeguentEntitat, null, Esdeveniment.Tipus.ARRIBADES);
        MotorSimuladorScript.Instancia.AfegirEsdeveniment(e);
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
                        etiquetes.text = "1/1\n"+transform.name;
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

    public override void ActualizaEstadistics(){
        string estadistics = "Output: " + nEntitatsGenerades+"\n";
        float tempsActual = (MotorSimuladorScript.Instancia.ObteTempsActual());
        if (estat == estats.BLOQUEJAT){
            tempsBloquejat += (tempsActual - ultimTemps); 
            etiquetes.text = "1/1\n";
        } 
        else {
            tempsGenerant += (tempsActual - ultimTemps);
            etiquetes.text = "0/1\n";
        }
        etiquetes.text += transform.name + "\n";
        ultimTemps = tempsActual;
        float tempsTotal = tempsGenerant+tempsBloquejat;
        float percGenerant = (float)Math.Round(100*(tempsGenerant/(tempsTotal)),2);
        float percBloquejat = (float)Math.Round(100-percGenerant,2);
        if (nEntitatsGenerades != 0) estadistics += "Temps entre entitats: " + (float)Math.Round(tempsTotal/nEntitatsGenerades) +"\n";
        estadistics += "% Generant: " + percGenerant + "\n";
        estadistics += "% Bloquejat: " + percBloquejat + "\n";

        
        etiquetes.text += estadistics; 

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

    public void ActualitzaPropietats(politiquesEnrutament novaPolitica, distribucionsProbabilitat d, float[] nousParametres, string nom, int entitatsTemp){
        transform.name = nom;
        etiquetes.text = "0/1\n"+transform.name;
        enrutament = novaPolitica;
        distribucio = d;
        entitatTemporal = entitatsTemporals[entitatsTemp];
        double[] aux = new double[nousParametres.Length];
        for (int i = 0; i < nousParametres.Length; ++i) aux[i] = (double)nousParametres[i];
        parametres = aux;
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
        posicioRatoliOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MotorSimuladorScript.Instancia.AlgunDetallsObert())
        {
            MotorSimuladorScript.Instancia.TancaDetallsObert();
        }
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) MotorSimuladorScript.Instancia.EliminarObjecte(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 7){
            MotorSimuladorScript.Instancia.ObreDetallsFill(transform.GetSiblingIndex());
        }
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);
    }

    private void OnMouseDrag(){
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + posicioRatoliOffset;
        for (int i = 0; i < SeguentsObjectes.Count; i++){
            LineRenderer lr = gameObject.transform.GetChild(2+i).GetComponent<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, SeguentsObjectes[i].transform.position);
        }
        for (int i = 0; i < ObjectesPredecessors.Count; i++){
            ObjectesPredecessors[i].GetComponent<LlibreriaObjectes>().CanviaPosicioPredecessor(this.gameObject);
        }
    }

}