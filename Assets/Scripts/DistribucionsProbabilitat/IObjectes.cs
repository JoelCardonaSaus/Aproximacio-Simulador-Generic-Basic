using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObjectes
{
    void recieveObject(GameObject entity); 
    void setTimeScale(float timeScale); 
    void ObreDetalls();
    void TancaDetalls();
    bool isAvailable();
    bool RatoliSobreDetalls();
    int sendObject();
    // Generador == 0; Cua == 1; Processador == 2; Sortida == 3; FakeObject == -1(per fer testing)
    int ObteTipusObjecte();
}
