using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortidaScript : LlibreriaObjectes
{
    private int nEntitatsDestruides;
    private List<double> tempsEntreEntitats;


    void Start()
    {
        nEntitatsDestruides = 0;
    }

    void Update()
    {
    }

    public override void IniciaSimulacio(){
        nEntitatsDestruides = 0;
        tempsEntreEntitats = new List<double>();
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        Debug.Log("Es destrueix una nova entitat");
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        ++nEntitatsDestruides;
        float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
        if (tempsEntreEntitats.Count != 0) {
            tempsEntreEntitats.Add(tActual-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
        } else {
            tempsEntreEntitats.Add(tActual);
        }
        Destroy(entitat);        
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        return false;
    }

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        return true;
    } 

    new public int CercaDisponible(){   
        return -1;
    }

    new public void AfegeixSeguentObjecte(GameObject objecte){
        
    }

    new public void IntentaEliminarObjecteSeguents(GameObject objecte){    }

    new public void DesajuntarSeguentObjecte(GameObject desjuntar){
        
    }

    public override int ObteTipusObjecte()
    {
        return 3;
    }
    

    public int getNEntitatsDestruides(){
        return nEntitatsDestruides;
    }


    public void InicialitzaPerFerTests(){
        tempsEntreEntitats.Add(0); // Creem el temps d'espera per la primera entitat
        nEntitatsDestruides = 0;
    }

    public override void GenerarPlots(){
        EstadisticsController eC = transform.parent.GetComponent<EstadisticsController>();
        double[] nEntitatsEstadistic = new double[1] { nEntitatsDestruides };
        string [] etiquetes = new string[1] { gameObject.transform.name };
        string nomImatge = "Output"+gameObject.transform.name;
        eC.GeneraEstadistic(0, nEntitatsEstadistic, etiquetes, "Sortides",nomImatge);
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
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) motorScript.EliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);
    }

    public override void ObreDetalls(){
        //gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public override void TancaDetalls(){
        //gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public override bool RatoliSobreDetalls(){
        return false;
    }
}
