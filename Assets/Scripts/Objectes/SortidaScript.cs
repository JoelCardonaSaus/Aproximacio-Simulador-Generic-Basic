using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortidaScript : MonoBehaviour, IObjectes
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

    public void IniciaSimulacio(){
        nEntitatsDestruides = 0;
        tempsEntreEntitats = new List<double>();
    }

    public void intentaEliminarObjecteSeguents(GameObject objecte){    }

    public bool estaDisponible(GameObject objecteLlibreria)
    {
        return true;
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
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

    public int cercaDisponible(){   
        return -1;
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public bool notificacioDisponible(GameObject objecteLlibreria)
    {
        return false;
    }

    public int getNEntitatsDestruides(){
        return nEntitatsDestruides;
    }

    public void afegeixSeguentObjecte(GameObject objecte){
        
    }

    public void desajuntarSeguentObjecte(GameObject desjuntar){
        
    }

    public void inicialitzaPerFerTests(){
        tempsEntreEntitats.Add(0); // Creem el temps d'espera per la primera entitat
        nEntitatsDestruides = 0;
    }

    public void GenerarPlots(){
        // Per implementar
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
        else if (UIScript.Instancia.obteBotoSeleccionat() == 4) UIScript.Instancia.ajuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 5) UIScript.Instancia.desjuntarObjectes(this.gameObject);
    }

    public void ObreDetalls(){
        //gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        //gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool RatoliSobreDetalls(){
        return false;
    }

    public int ObteTipusObjecte()
    {
        return 3;
    }
}
