using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorSimuladorScript : MonoBehaviour
{
    public float escalaTemps;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objectesLlibreria = GameObject.FindGameObjectsWithTag("ObjecteLlibreria");
        for (int i = 0; i < objectesLlibreria.Length; i++) {
            objectesLlibreria[i].GetComponent<IRebreObjecte>().setTimeScale(escalaTemps);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate() {
        if (escalaTemps < 0.5f) escalaTemps = 0.5f;
        else if (escalaTemps > 10f) escalaTemps = 10f;     
    }
}
