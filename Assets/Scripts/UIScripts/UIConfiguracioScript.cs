using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConfiguracioScript : MonoBehaviour
{
    public InputField tempsMaxim;
    public Button cancela;
    public Button aplicar;
    private float tempsMaximActual = 86400;
    private float tempsMaximConfirmat = 86400;
    public GameObject finestraErrors;

    public void CanviTempsMaxim(){
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            float aux;
            if (float.TryParse(tempsMaxim.text, out aux)){
                if (aux <= 0){
                    UIScript.Instancia.MostrarError("ERROR: El temps màxim ha de ser superior a 0");
                }
                tempsMaximActual = aux;
                cancela.interactable = true;
                aplicar.interactable = true;
            } else {
                UIScript.Instancia.MostrarError("ERROR: El temps indicat no és un temps vàlid. Si es vol passar un temps no enter s'ha de fer amb el càracter ','");
            }
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void AplicarCanvis(){
        if (tempsMaximActual != tempsMaximConfirmat){
            tempsMaximConfirmat = tempsMaximActual;
            MotorSimuladorScript.Instancia.CanviaTempsMaxim(tempsMaximConfirmat);
        }
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    public void CancelaCanvis(){
        tempsMaximActual = tempsMaximConfirmat;
        tempsMaxim.text = tempsMaximActual.ToString();
        
        aplicar.interactable = false;
        cancela.interactable = false;
    }

    void Start()
    {
        finestraErrors = GameObject.Find("FinestraErrors");
    }

    void Update()
    {
        
    }
}
