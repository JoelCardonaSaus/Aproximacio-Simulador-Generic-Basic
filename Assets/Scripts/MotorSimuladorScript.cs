using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// Fer-ho singleton
public class MotorSimuladorScript : MonoBehaviour
{
    public float escalaTemps;

    private List<GameObject> objectesLlibreria = new List<GameObject>();
    private List<Esdeveniment> llistaEsdevenmients = new List<Esdeveniment>();

    private float tempsActual;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject[] objectesLlibreria = GameObject.FindGameObjectsWithTag("ObjecteLlibreria");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (llistaEsdevenmients.Count > 0){
            Esdeveniment eActual = llistaEsdevenmients[0];
            llistaEsdevenmients.Remove(eActual);
            tempsActual = eActual.temps;
            eActual.Executar();
        }
        
    }

    public void afegirEsdeveniment(Esdeveniment nouEsdeveminemt){
        llistaEsdevenmients.Add(nouEsdeveminemt);
        llistaEsdevenmients.Sort((e1, e2)=>e1.temps.CompareTo(e2.temps));
    }
    public void afegirObjecteLlista(GameObject nouObjecte){
        objectesLlibreria.Add(nouObjecte);
        nouObjecte.GetComponent<GeneradorScript>().IniciaSimulacio();
        Debug.Log("estic aqui");
    }
    private void OnValidate() {
        if (escalaTemps < 0.5f) escalaTemps = 0.5f;
        else if (escalaTemps > 10f) escalaTemps = 10f;     
    }
}
