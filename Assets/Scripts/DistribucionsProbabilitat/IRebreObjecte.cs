using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRebreObjecte
{
    void recieveObject(GameObject entity); 
    bool isAvailable();
    bool sendObject();
    void setTimeScale(float timeScale); 
}
