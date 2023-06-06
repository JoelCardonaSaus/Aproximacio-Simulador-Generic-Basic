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
    private float[] parametresActuals;
    private float[] parametresConfirmats;
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

    public void CanviEnrutamentSeleccionat()
    {
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
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
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            cancela.interactable = true;
            aplicar.interactable = true;
            if (distribuidor.value == 1){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.BINOMIAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Probabilitat (0, 1]:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "# intents (int):";
                parametresActuals = new float[2];
                iParam1.text = ""; iParam2.text = "";
            } 
            else if (distribuidor.value == 0){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.CONSTANT;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps constant:";
                parametresActuals = new float[1];
                iParam1.text = "";
            }
            else if (distribuidor.value == 2){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.EXPONENTIAL;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps entre arribades:";
                parametresActuals = new float[1];
                iParam1.text = "";
            } 
            else if (distribuidor.value == 3){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.NORMAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mitjana:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Desviació estàndard:";
                parametresActuals = new float[2];
                iParam1.text = ""; iParam2.text = "";
            } 
            else if (distribuidor.value == 4) {
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.POISSON;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lambda:";
                parametresActuals = new float[1];
                parametresActuals[0] = 1;
                parametresConfirmats = parametresActuals;
                iParam1.text = "1";
            }
            else if (distribuidor.value == 5){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.TRIANGULAR;
                param2.SetActive(true); param3.SetActive(true);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mínim:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Màxim:";
                param3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mode:";
                parametresActuals = new float[3];    
                iParam1.text = ""; iParam2.text = ""; iParam3.text = "";            
            } 
            else if (distribuidor.value == 6){
                distribucioActual = ProcessadorScript.distribucionsProbabilitat.DISCRETEUNIFORM;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mínim:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Màxim:";
                parametresActuals = new float[2];
                iParam1.text = ""; iParam2.text = "";
            } 
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviaParametre1(){
        CanviaParametres(1);
    }

    public void CanviaParametre2(){
        CanviaParametres(2);
    }

    public void CanviaParametre3(){
        CanviaParametres(3);
    }

    public void CanviaParametres(int p){
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            if (p == 1){
                float aux;
                if (float.TryParse(iParam1.text, out aux)){
                    if (aux < 0) UIScript.Instancia.MostrarError("ERROR: El primer paràmetre ha de ser major o igual a 0");
                    else {
                        if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.BINOMIAL){
                            if (aux > 1) UIScript.Instancia.MostrarError("ERROR: El primer paràmetre ha d'estar inclós entre 0 i 1");
                            else{
                                parametresActuals[0] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }
                        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.POISSON){
                            if (aux <= 0) UIScript.Instancia.MostrarError("ERROR: La lambda ha de ser superior a 0");
                            else{
                                parametresActuals[0] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }  
                        else if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.TRIANGULAR || distribucioActual == ProcessadorScript.distribucionsProbabilitat.DISCRETEUNIFORM) {
                            if (parametresActuals[1] != 0 && parametresActuals[1] < aux) UIScript.Instancia.MostrarError("ERROR: El primer paràmetre no pot ser major que el segon paràmetre");
                            else{
                                parametresActuals[0] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }
                        else{
                            parametresActuals[0] = aux;
                            aplicar.interactable = true;
                            cancela.interactable = true;
                        }
                    }
                } else {
                    UIScript.Instancia.MostrarError("ERROR: El primer paràmetre ha de ser un número");
                }
            }
            else if (p == 2){
                if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.BINOMIAL){
                    int aux;
                    if (int.TryParse(iParam2.text, out aux)){
                        if (aux < 0) UIScript.Instancia.MostrarError("ERROR: El segon paràmetre ha de ser major a 0");
                        else {
                            aplicar.interactable = true;
                            cancela.interactable = true;
                            parametresActuals[1] = aux;
                        }
                    }
                    else {
                        UIScript.Instancia.MostrarError("ERROR: El segon paràmetre ha de ser un número enter");
                    }
                } else {
                    float aux;
                    if (float.TryParse(iParam2.text, out aux)){
                        if (aux < 0) UIScript.Instancia.MostrarError("ERROR: El segon paràmetre ha de ser major o igual a 0");
                        else {
                            if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.TRIANGULAR || distribucioActual == ProcessadorScript.distribucionsProbabilitat.DISCRETEUNIFORM) {
                                if (parametresActuals[0] != 0 && parametresActuals[0] > aux) UIScript.Instancia.MostrarError("ERROR: El primer paràmetre no pot ser major que el segon paràmetre");
                                else{
                                    parametresActuals[1] = aux;
                                    aplicar.interactable = true;
                                    cancela.interactable = true;
                                }
                            }
                            else{
                                parametresActuals[1] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }
                    }
                }
            } else {
                float aux;
                if (float.TryParse(iParam3.text, out aux)){
                    if (aux < 0) UIScript.Instancia.MostrarError("ERROR: El tercer paràmetre ha de ser major o igual a 0");
                    else {
                        if (distribucioActual == ProcessadorScript.distribucionsProbabilitat.TRIANGULAR) {
                            if ((parametresActuals[0] != 0 && parametresActuals[0] > aux) || (parametresActuals[1] != 0 && parametresActuals[1] < aux)) UIScript.Instancia.MostrarError("ERROR: El tercer paràmetre ha d'estar inclós entre el primer i el segon paràmetre");
                            else{
                                parametresActuals[2] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }
                        else{
                            parametresActuals[2] = aux;
                            aplicar.interactable = true;
                            cancela.interactable = true;
                        }
                    }
                }
            }
        } else {
            aplicar.interactable = false;
            cancela.interactable = false;
        }
    }

    public void CanviCapacitat(){
        if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT)
        {
            int aux;
            if (int.TryParse(capacitatInput.text, out aux)){
                if (aux <= 0 && aux != -1) UIScript.Instancia.MostrarError("ERROR: La capacitat del processador ha de ser superior a 0, o -1 en cas d'una capacitat infinita");
                else {
                    capacitatActual = aux;
                    cancela.interactable = true;
                    aplicar.interactable = true;
                }
            } else {
                UIScript.Instancia.MostrarError("ERROR: La capacitat màxima del processador ha de ser un número enter");
            }
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

        parametresConfirmats = new float[1]{5};
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
