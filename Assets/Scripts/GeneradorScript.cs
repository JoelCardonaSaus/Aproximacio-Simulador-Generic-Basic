using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorScript : MonoBehaviour
{

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    public enum distribucionsProbabilitat { CHIQUADRAT, EXPONENTIAL, FISHER, NORMAL, POISSON, TRIANGULAR, TSTUDENT };
    public distribucionsProbabilitat distribucio;
    //public double[] parametres;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate() {
        Debug.Log("I am on validate variable");    
    }
}