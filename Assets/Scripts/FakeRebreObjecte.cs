using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRebreObjecte : MonoBehaviour, IObjectes
{
    private void Start() {
        
    }

    private void Update() {
        
    }

    public void IniciaSimulacio() {}

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        
    } 

    public bool estaDisponible(GameObject objecteLlibreria)
    {
        return true;
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
     
    }

    public int cercaDisponible(){   
        return -1;
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public bool notificacioDisponible(GameObject objecteLlibreria)
    {
        return false;
    }

    public void ObreDetalls(){
        
    }   

    public void TancaDetalls(){
        
    }

    public bool RatoliSobreDetalls(){
        return false;
    }

    public int ObteTipusObjecte()
    {
        return -1;
    }

    public void afegeixSeguentObjecte(GameObject objecte){
        
    }

    public void desajuntarSeguentObjecte(GameObject desjuntar){
        
    }

    public void GenerarPlots(){

    }

}
