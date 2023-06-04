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
    private string nomActual;
    private string nomConfirmat;
    public InputField nomObjecte;

    public Dropdown enrutament; 
    public InputField capacitatInput;
    public Button cancela;
    public Button aplicar;

    public void CanviaNom()
    {
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            nomActual = nomObjecte.text;
            
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviEnrutamentSeleccionat()
    {
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            if (enrutament.value == 0) politicaActual = CuaScript.politiquesEnrutament.PRIMERDISPONIBLE;
            else if (enrutament.value == 1) politicaActual = CuaScript.politiquesEnrutament.RANDOM;
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviCapacitat(){
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            int aux;
            if (int.TryParse(capacitatInput.text, out aux)){
                if (aux <= 0 && aux != -1) UIScript.Instancia.MostrarError("ERROR: La capacitat de la cua ha de ser superior a 0, o -1 en cas d'una capacitat infinita");
                else {
                    capacitatActual = aux;
                    cancela.interactable = true;
                    aplicar.interactable = true;
                }
            } else {
                UIScript.Instancia.MostrarError("ERROR: La capacitat màxima de la cua ha de ser un número enter");
            }
            
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void AplicarCanvis(){
        cuaScript.ActualitzaPropietats(politicaActual, capacitatActual, nomActual);
        politicaConfirmada = politicaActual;
        capacitatConfirmada = capacitatActual;
        nomConfirmat = nomActual;
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        politicaActual = politicaConfirmada;
        capacitatActual = capacitatConfirmada;
        nomActual = nomConfirmat;

        if (politicaConfirmada == CuaScript.politiquesEnrutament.PRIMERDISPONIBLE) enrutament.value = 0;
        else if (politicaConfirmada == CuaScript.politiquesEnrutament.RANDOM) enrutament.value = 1;

        capacitatInput.text = capacitatActual.ToString();
        
    }

    void Start()
    {
        cuaScript = gameObject.transform.parent.GetComponentInParent<CuaScript>();
        politicaConfirmada = CuaScript.politiquesEnrutament.PRIMERDISPONIBLE;
        politicaActual = politicaConfirmada;

        capacitatConfirmada = -1;
        capacitatActual = capacitatConfirmada;

        nomObjecte.text = gameObject.transform.parent.transform.parent.name;
        nomConfirmat = gameObject.transform.parent.transform.parent.name;
        nomActual = nomConfirmat;
        
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
