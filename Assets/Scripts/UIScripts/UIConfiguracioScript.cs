using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConfiguracioScript : MonoBehaviour
{
    public InputField tempsMaxim;
    public Dropdown entitat;
    public Button cancela;
    public Button aplicar;
    private enum entitats { PAQUETS, PERSONES, CERCLES };
    private entitats entitatsActuals = entitats.PAQUETS;
    private entitats entitatsConfirmades = entitats.PAQUETS;
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

    public void CanviEntitatsSeleccionat()
    {
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            if (entitat.value == 0) entitatsActuals = entitats.PAQUETS;
            else if (entitat.value == 1) entitatsActuals = entitats.PERSONES;
            else if (entitat.value == 2) entitatsActuals = entitats.CERCLES;
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void AplicarCanvis(){
        if (entitatsActuals != entitatsConfirmades){
            entitatsConfirmades = entitatsActuals;
            MotorSimuladorScript.Instancia.CanviaEntitatsTemporals((int)entitatsConfirmades);
        }
        if (tempsMaximActual != tempsMaximConfirmat){
            tempsMaximConfirmat = tempsMaximActual;
            MotorSimuladorScript.Instancia.CanviaTempsMaxim(tempsMaximConfirmat);
        }
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    public void CancelaCanvis(){
        entitatsActuals = entitatsConfirmades;
        tempsMaximActual = tempsMaximConfirmat;
        tempsMaxim.text = tempsMaximActual.ToString();
        if (entitatsConfirmades == entitats.PAQUETS) entitat.value = 0;
        else if (entitatsConfirmades == entitats.PERSONES) entitat.value = 1;
        else if (entitatsConfirmades == entitats.CERCLES) entitat.value = 2;
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
