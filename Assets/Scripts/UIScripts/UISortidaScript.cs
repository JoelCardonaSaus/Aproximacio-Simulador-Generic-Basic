using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISortidaScript : MonoBehaviour
{
    private string nomActual;
    private string nomConfirmat;
    public InputField nomObjecte;
    public Button aplicar;
    public Button cancela;
    public SortidaScript sortidaScript;


    public void CanviaNom()
    {
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            nomActual = nomObjecte.text;
            
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void AplicarCanvis(){
        nomConfirmat = nomActual;
        cancela.interactable = false;
        aplicar.interactable = false;
        sortidaScript.ActualitzaPropietats(nomActual);
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        
        nomActual = nomConfirmat;
    }

    void Start()
    {
        sortidaScript = gameObject.transform.parent.GetComponentInParent<SortidaScript>();
        nomObjecte.text = gameObject.transform.parent.transform.parent.name;
        nomConfirmat = gameObject.transform.parent.transform.parent.name;
        nomActual = nomConfirmat;

        sortidaScript.ActualitzaPropietats(nomConfirmat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
