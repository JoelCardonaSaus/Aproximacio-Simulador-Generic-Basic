using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortidaScript : MonoBehaviour, IRebreObjecte
{
    private int nEntitatsDestruides;
    private List<double> tempsEntreEntitats = new List<double>();
    // Start is called before the first frame update
    void Start()
    {
        tempsEntreEntitats.Add(0); // Creem el temps d'espera per la primera entitat
        nEntitatsDestruides = 0;
    }

    // Update is called once per frame
    void Update()
    {
        tempsEntreEntitats[tempsEntreEntitats.Count-1] += Time.deltaTime;
    }

    public bool isAvailable()
    {
        return true;
    }

    public void recieveObject(GameObject entity)
    {
        entity.transform.position = transform.position + new Vector3(0,+1,0);
        Debug.Log("Temps de l'entitat: " + tempsEntreEntitats[tempsEntreEntitats.Count-1]);
        ++nEntitatsDestruides;
        tempsEntreEntitats.Add(0);

        // Recolectar estadistics de la entitat abans de destruirla!
        Destroy(entity);
    }

    public bool sendObject()
    {
        return false;
    }
}
