using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentCamara : MonoBehaviour
{
    public float velocitatCamara = 10f;
    private Vector3 ultimaPosicio;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ultimaPosicio = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            
            ultimaPosicio = Input.mousePosition - ultimaPosicio;

            transform.position -= ultimaPosicio * velocitatCamara * Time.deltaTime;

            ultimaPosicio = Input.mousePosition;
        }
    }
}
