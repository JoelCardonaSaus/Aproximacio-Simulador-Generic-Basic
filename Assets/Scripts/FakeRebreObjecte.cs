using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRebreObjecte : MonoBehaviour, IObjectes
{
    private void Start() {
        
    }

    private void Update() {
        
    }
    public void setTimeScale(float timeScale){
        
    }

    public int sendObject(){
        return 0;
    }

    public bool isAvailable(GameObject objectePropietari){
        return true;
    }
    
    public bool recieveObject(GameObject entity, float tempsActual){
        return true;
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
}
