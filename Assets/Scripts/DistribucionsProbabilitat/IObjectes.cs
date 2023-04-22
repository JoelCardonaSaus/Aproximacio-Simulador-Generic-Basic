using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObjectes
{
    void recieveObject(GameObject entity); 
    bool isAvailable();
    bool sendObject();
    void setTimeScale(float timeScale); 
    void ObreDetalls();
    void TancaDetalls();
    bool RatoliSobreDetalls();
}
