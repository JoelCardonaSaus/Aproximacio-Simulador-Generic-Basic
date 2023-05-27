using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIProcessadorScript : MonoBehaviour
{
    private ProcessadorScript.politiquesEnrutament politicaActual;
    private ProcessadorScript.politiquesEnrutament politicaConfirmada;
    private ProcessadorScript.distribucionsProbabilitat distribucioActual;
    private ProcessadorScript.distribucionsProbabilitat distribucioConfirmada;
    private string nomActual;
    private string nomConfirmat;
    private double[] parametresActuals;
    private double[] parametresConfirmats;
    public InputField nomObjecte;
    public Dropdown enrutament; 
    public Dropdown distribuidor;
    public InputField iParam1;
    public InputField iParam2;
    public InputField iParam3;
    public GameObject param1;
    public GameObject param2;
    public GameObject param3;
    private int capacitatActual;
    private int capacitatConfirmada;
    public InputField capacitatInput;
    private ProcessadorScript processadorScript;
    public Button aplicar;
    public Button cancela;

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
            if (enrutament.value == 0) politicaActual = ProcessadorScript.politiquesEnrutament.PRIMERDISPONIBLE;
            else if (enrutament.value == 1) politicaActual = ProcessadorScript.politiquesEnrutament.RANDOM;
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviDistribuidorSeleccionat()
    {
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            if (distribuidor.value == 1){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.BINOMIAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Probabilitat (0, 1]:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "# intents (int):";
            } 
            else if (distribuidor.value == 0){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.CONSTANT;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps constant:";
            }
            else if (distribuidor.value == 2){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.EXPONENTIAL;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps entre arribades:";
            } 
            else if (distribuidor.value == 3){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.NORMAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mitjana:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Desviació estàndard:";
            } 
            else if (distribuidor.value == 4) {
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.POISSON;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "# events per unitat de temps:";
            }
            else if (distribuidor.value == 5){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.TRIANGULAR;
                param2.SetActive(true); param3.SetActive(true);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mínim:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Màxim:";
                param3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mode:";                
            } 
            else if (distribuidor.value == 6){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.DISCRETEUNIFORM;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mínim:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Màxim:";
            } 
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviaParametres(){
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            aplicar.interactable = true;
            cancela.interactable = true;
            if (param2.activeSelf && param3.activeSelf){
                parametresActuals = new double[3];
                parametresActuals[0] = Double.Parse(iParam1.text);
                parametresActuals[1] = Double.Parse(iParam2.text);
                parametresActuals[2] = Double.Parse(iParam3.text);
            } 
            else if (param2.activeSelf && !param3.activeSelf){
                parametresActuals = new double[2];
                parametresActuals[0] = Double.Parse(iParam1.text);
                parametresActuals[1] = Double.Parse(iParam2.text);
            }
            else {
                parametresActuals = new double[1];
                parametresActuals[0] = Double.Parse(iParam1.text);
            }
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviCapacitat(){
        if (UIScript.Instancia.ObteEstatSimulador() == 1)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            capacitatActual = int.Parse(capacitatInput.text);
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void AplicarCanvis(){
        politicaConfirmada = politicaActual;
        distribucioConfirmada = distribucioActual;
        parametresConfirmats = parametresActuals;
        capacitatConfirmada = capacitatActual;
        nomConfirmat = nomActual;
        cancela.interactable = false;
        aplicar.interactable = false;
        processadorScript.ActualitzaPropietats(politicaActual, distribucioActual, parametresActuals, capacitatActual, nomActual);
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        politicaActual = politicaConfirmada;
        distribucioActual = distribucioConfirmada;
        parametresActuals = parametresConfirmats;
        capacitatActual = capacitatConfirmada;
        nomActual = nomConfirmat;

        if (politicaConfirmada == ProcessadorScript.politiquesEnrutament.PRIMERDISPONIBLE) enrutament.value = 0;
        else if (politicaConfirmada == ProcessadorScript.politiquesEnrutament.RANDOM) enrutament.value = 1;

        if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.BINOMIAL) distribuidor.value = 1;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.CONSTANT) distribuidor.value = 0;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.EXPONENTIAL) distribuidor.value = 2;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.NORMAL) distribuidor.value = 3;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.POISSON) distribuidor.value = 4;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.TRIANGULAR) distribuidor.value = 5;
        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.DISCRETEUNIFORM) distribuidor.value = 6;
        
        if (distribuidor.value == 0 || distribuidor.value == 2 || distribuidor.value == 4){
            param2.SetActive(false); param3.SetActive(false);
            iParam1.text = parametresConfirmats[0].ToString();
        }
        else if (distribuidor.value == 3 || distribuidor.value == 1 || distribuidor.value == 6){
            param2.SetActive(true); param3.SetActive(false);
            iParam1.text = parametresConfirmats[0].ToString();
            iParam2.text = parametresConfirmats[1].ToString();

        } else {
            param2.SetActive(true); param3.SetActive(true);
            iParam1.text = parametresConfirmats[0].ToString();
            iParam2.text = parametresConfirmats[1].ToString();
            iParam3.text = parametresConfirmats[2].ToString();
        }

        capacitatInput.text = capacitatActual.ToString();
    }

    void Start()
    {
        processadorScript = gameObject.transform.parent.GetComponentInParent<ProcessadorScript>();
        
        distribucioConfirmada = ProcessadorScript.distribucionsProbabilitat.CONSTANT;
        distribucioActual = distribucioConfirmada;

        politicaConfirmada = ProcessadorScript.politiquesEnrutament.PRIMERDISPONIBLE;
        politicaActual = politicaConfirmada;

        parametresConfirmats = new double[1]{5};
        parametresActuals = parametresConfirmats;

        capacitatConfirmada = -1;
        capacitatActual = capacitatConfirmada;

        nomObjecte.text = gameObject.transform.parent.transform.parent.name;
        nomConfirmat = gameObject.transform.parent.transform.parent.name;
        nomActual = nomConfirmat;
        
        processadorScript.ActualitzaPropietats(politicaConfirmada, distribucioConfirmada, parametresConfirmats, capacitatConfirmada, nomConfirmat);
    }

    void Update()
    {

    }
}
