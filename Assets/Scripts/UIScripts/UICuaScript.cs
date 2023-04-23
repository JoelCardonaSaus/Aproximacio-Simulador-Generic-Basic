using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICuaScript : MonoBehaviour
{
    private CuaScript cuaScript;
    private CuaScript.politiquesEnrutament politicaActual;
    private CuaScript.politiquesEnrutament politicaConfirmada;
    private int capacitatActual;
    private int capacitatConfirmada;
    public Dropdown enrutament; 
    public InputField capacitatInput;
    public Button cancela;
    public Button aplicar;


    public void CanviEnrutamentSeleccionat()
    {
        cancela.interactable = true;
        aplicar.interactable = true;
        if (enrutament.value == 0) politicaActual = CuaScript.politiquesEnrutament.PRIMERDISPONIBLE;
        else if (enrutament.value == 1) politicaActual = CuaScript.politiquesEnrutament.RANDOM;
    }

    public void CanviCapacitat(){
        cancela.interactable = true;
        aplicar.interactable = true;
        capacitatActual = Convert.ToInt32(capacitatInput.text);
    }

    public void AplicarCanvis(){
        cuaScript.ActualitzaPropietats(politicaActual, capacitatActual);
        politicaConfirmada = politicaActual;
        capacitatConfirmada = capacitatActual;
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        politicaActual = politicaConfirmada;
        capacitatActual = capacitatConfirmada;

        if (politicaConfirmada == CuaScript.politiquesEnrutament.PRIMERDISPONIBLE) enrutament.value = 0;
        else if (politicaConfirmada == CuaScript.politiquesEnrutament.RANDOM) enrutament.value = 1;

        capacitatInput.text = capacitatActual.ToString();
        
    }

    void Start()
    {
        cuaScript = gameObject.transform.parent.GetComponentInParent<CuaScript>();
        politicaConfirmada = CuaScript.politiquesEnrutament.PRIMERDISPONIBLE;
        capacitatConfirmada = -1;
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
