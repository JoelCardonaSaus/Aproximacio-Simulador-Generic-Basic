using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGeneradorScript : MonoBehaviour
{
    private GeneradorScript.politiquesEnrutament politicaActual;
    private GeneradorScript.politiquesEnrutament politicaConfirmada;
    private GeneradorScript.distribucionsProbabilitat distribucioActual;
    private GeneradorScript.distribucionsProbabilitat distribucioConfirmada;
    private string nomActual;
    private string nomConfirmat;
    public Dropdown entitat;

    private enum entitats { PAQUETS, PERSONES, CERCLES };
    private entitats entitatsActuals = entitats.PAQUETS;
    private entitats entitatsConfirmades = entitats.PAQUETS;
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
    public GeneradorScript generadorScript;
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
            if (enrutament.value == 0) politicaActual = GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE;
            else if (enrutament.value == 1) politicaActual = GeneradorScript.politiquesEnrutament.RANDOM;
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
                distribucioActual = GeneradorScript.distribucionsProbabilitat.BINOMIAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Probabilitat (0, 1]:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "# intents (int):";
                parametresActuals = new float[2];
                iParam1.text = ""; iParam2.text = "";
            } 
            else if (distribuidor.value == 0){
                distribucioActual = GeneradorScript.distribucionsProbabilitat.CONSTANT;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps constant:";
                parametresActuals = new float[1];
                iParam1.text = "";
            }
            else if (distribuidor.value == 2){
                distribucioActual = GeneradorScript.distribucionsProbabilitat.EXPONENTIAL;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Temps entre arribades:";
                parametresActuals = new float[1];
                iParam1.text = "";
            } 
            else if (distribuidor.value == 3){
                distribucioActual = GeneradorScript.distribucionsProbabilitat.NORMAL;
                param2.SetActive(true); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mitjana:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Desviació estàndard:";
                parametresActuals = new float[2];
                iParam1.text = ""; iParam2.text = "";
            } 
            else if (distribuidor.value == 4) {
                distribucioActual = GeneradorScript.distribucionsProbabilitat.POISSON;
                param2.SetActive(false); param3.SetActive(false);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lambda:";
                parametresActuals = new float[1];
                parametresActuals[0] = 1;
                parametresConfirmats = parametresActuals;
                iParam1.text = "1";
            }
            else if (distribuidor.value == 5){
                distribucioActual = GeneradorScript.distribucionsProbabilitat.TRIANGULAR;
                param2.SetActive(true); param3.SetActive(true);
                param1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mínim:";
                param2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Màxim:";
                param3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mode:";
                parametresActuals = new float[3];  
                iParam1.text = ""; iParam2.text = ""; iParam3.text = "";              
            } 
            else if (distribuidor.value == 6){
                distribucioActual = GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM;
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
                        if (distribucioActual == GeneradorScript.distribucionsProbabilitat.BINOMIAL){
                            if (aux > 1) UIScript.Instancia.MostrarError("ERROR: El primer paràmetre ha d'estar inclós entre 0 i 1");
                            else{
                                parametresActuals[0] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }
                        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.POISSON){
                            if (aux <= 0) UIScript.Instancia.MostrarError("ERROR: La lambda ha de ser superior a 0");
                            else{
                                parametresActuals[0] = aux;
                                aplicar.interactable = true;
                                cancela.interactable = true;
                            }
                        }  
                        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.TRIANGULAR || distribucioActual == GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM) {
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
                if (distribucioActual == GeneradorScript.distribucionsProbabilitat.BINOMIAL){
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
                            if (distribucioActual == GeneradorScript.distribucionsProbabilitat.TRIANGULAR || distribucioActual == GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM) {
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
                        if (distribucioActual == GeneradorScript.distribucionsProbabilitat.TRIANGULAR) {
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

    public void AplicarCanvis(){
        politicaConfirmada = politicaActual;
        distribucioConfirmada = distribucioActual;
        parametresConfirmats = parametresActuals;
        nomConfirmat = nomActual;
        if (entitatsActuals != entitatsConfirmades){
            entitatsConfirmades = entitatsActuals;
        }
        cancela.interactable = false;
        aplicar.interactable = false;
        generadorScript.ActualitzaPropietats(politicaActual, distribucioActual, parametresActuals, nomActual, (int)entitatsConfirmades);
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        politicaActual = politicaConfirmada;
        distribucioActual = distribucioConfirmada;
        parametresActuals = parametresConfirmats;
        nomActual = nomConfirmat;

        if (politicaConfirmada == GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE) enrutament.value = 0;
        else if (politicaConfirmada == GeneradorScript.politiquesEnrutament.RANDOM) enrutament.value = 1;

        if (distribucioActual == GeneradorScript.distribucionsProbabilitat.BINOMIAL) distribuidor.value = 1;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.CONSTANT) distribuidor.value = 0;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.EXPONENTIAL) distribuidor.value = 2;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.NORMAL) distribuidor.value = 3;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.POISSON) distribuidor.value = 4;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.TRIANGULAR) distribuidor.value = 5;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM) distribuidor.value = 6;
        
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

        entitatsActuals = entitatsConfirmades;
        if (entitatsConfirmades == entitats.PAQUETS) entitat.value = 0;
        else if (entitatsConfirmades == entitats.PERSONES) entitat.value = 1;
        else if (entitatsConfirmades == entitats.CERCLES) entitat.value = 2;
    }

    void Start()
    {
        generadorScript = gameObject.transform.parent.GetComponentInParent<GeneradorScript>();
        distribucioConfirmada = GeneradorScript.distribucionsProbabilitat.CONSTANT;
        distribucioActual = distribucioConfirmada;
        politicaConfirmada = GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE;
        parametresConfirmats = new float[1]{5};
        parametresActuals = parametresConfirmats;
        nomObjecte.text = gameObject.transform.parent.transform.parent.name;
        nomConfirmat = gameObject.transform.parent.transform.parent.name;
        nomActual = nomConfirmat;
    }

    void Update()
    {

    }
}
