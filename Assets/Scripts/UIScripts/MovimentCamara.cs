using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovimentCamara : MonoBehaviour
{
    public float velocitatCamara = 10f;
    private Vector3 ultimaPosicio;

    public Camera camara;
    public Slider zoomSlider;
    public float minZoom = 5f;
    public float maxZoom = 12.5f;

    void Start()
    {
        zoomSlider.onValueChanged.AddListener(SliderCanvia);
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

     private void SliderCanvia(float value)
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, value);
        camara.orthographicSize = zoom;
    }
}
