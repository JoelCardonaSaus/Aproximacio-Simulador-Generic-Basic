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
        Destroy(entitat, 1);        
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

    public void inicialitzaPerFerTests(){
        tempsEntreEntitats.Add(0); // Creem el temps d'espera per la primera entitat
        nEntitatsDestruides = 0;
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
