using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObjectes
{
    bool recieveObject(GameObject entity, float tempsActual); 
    void setTimeScale(float timeScale); 
    void ObreDetalls();
    void TancaDetalls();
    bool isAvailable(GameObject objectePropietari);
    bool RatoliSobreDetalls();
    int sendObject();
    // Generador == 0; Cua == 1; Processador == 2; Sortida == 3; FakeObject == -1(per fer testing)
    int ObteTipusObjecte();
}
