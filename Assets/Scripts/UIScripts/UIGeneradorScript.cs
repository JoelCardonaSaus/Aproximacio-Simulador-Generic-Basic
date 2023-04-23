using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGeneradorScript : MonoBehaviour
{
    private GeneradorScript.politiquesEnrutament politicaActual;
    private GeneradorScript.politiquesEnrutament politicaConfirmada;
    private GeneradorScript.distribucionsProbabilitat distribucioActual;
    private GeneradorScript.distribucionsProbabilitat distribucioConfirmada;
    private double[] parametresActuals;
    private double[] parametresConfirmats;
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
    public void CanviEnrutamentSeleccionat()
    {
        cancela.interactable = true;
        aplicar.interactable = true;
        if (enrutament.value == 0) politicaActual = GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE;
        else if (enrutament.value == 1) politicaActual = GeneradorScript.politiquesEnrutament.RANDOM;
    }

    public void CanviDistribuidorSeleccionat()
    {
        cancela.interactable = true;
        aplicar.interactable = true;
        if (distribuidor.value == 0) distribucioActual = GeneradorScript.distribucionsProbabilitat.BINOMIAL;
        else if (distribuidor.value == 1) distribucioActual = GeneradorScript.distribucionsProbabilitat.CONSTANT;
        else if (distribuidor.value == 2) distribucioActual = GeneradorScript.distribucionsProbabilitat.EXPONENTIAL;
        else if (distribuidor.value == 3) distribucioActual = GeneradorScript.distribucionsProbabilitat.NORMAL;
        else if (distribuidor.value == 4) distribucioActual = GeneradorScript.distribucionsProbabilitat.POISSON;
        else if (distribuidor.value == 5) distribucioActual = GeneradorScript.distribucionsProbabilitat.TRIANGULAR;
        else if (distribuidor.value == 6) distribucioActual = GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM;
        
        if (distribuidor.value == 1 || distribuidor.value == 2 || distribuidor.value == 4){
            param2.SetActive(false); param3.SetActive(false);
        }
        else if (distribuidor.value == 3 || distribuidor.value == 0 || distribuidor.value == 6){
            param2.SetActive(true); param3.SetActive(false);
        } else {
            param2.SetActive(true); param3.SetActive(true);
        }
    }

    public void CanviaParametres(){
        aplicar.interactable = true;
        cancela.interactable = true;
        if (param2.activeSelf && param3.activeSelf){
            parametresActuals = new double[3];
            parametresActuals[0] = Convert.ToDouble(iParam1.text);
            parametresActuals[1] = Convert.ToDouble(iParam2.text);
            parametresActuals[2] = Convert.ToDouble(iParam3.text);
        } 
        else if (param2.activeSelf && !param3.activeSelf){
            parametresActuals = new double[2];
            parametresActuals[0] = Convert.ToDouble(iParam1.text);
            parametresActuals[1] = Convert.ToDouble(iParam2.text);
        }
        else {
            parametresActuals = new double[1];
            parametresActuals[0] = Convert.ToDouble(iParam1);
        }
    }

    public void AplicarCanvis(){
        generadorScript.ActualitzaPropietats(politicaActual, distribucioActual, parametresActuals);
        politicaConfirmada = politicaActual;
        distribucioConfirmada = distribucioActual;
        parametresConfirmats = parametresActuals;
        cancela.interactable = false;
        aplicar.interactable = false;
    }

    public void CancelaCanvis(){
        aplicar.interactable = false;
        cancela.interactable = false;
        politicaActual = politicaConfirmada;
        distribucioActual = distribucioConfirmada;
        parametresActuals = parametresConfirmats;

        if (politicaConfirmada == GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE) enrutament.value = 0;
        else if (politicaConfirmada == GeneradorScript.politiquesEnrutament.RANDOM) enrutament.value = 1;

        if (distribucioActual == GeneradorScript.distribucionsProbabilitat.BINOMIAL) distribuidor.value = 0;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.CONSTANT) distribuidor.value = 1;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.EXPONENTIAL) distribuidor.value = 2;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.NORMAL) distribuidor.value = 3;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.POISSON) distribuidor.value = 4;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.TRIANGULAR) distribuidor.value = 5;
        else if (distribucioActual == GeneradorScript.distribucionsProbabilitat.DISCRETEUNIFORM) distribuidor.value = 6;
        
        if (distribuidor.value == 1 || distribuidor.value == 2 || distribuidor.value == 4){
            param2.SetActive(false); param3.SetActive(false);
            iParam1.text = parametresConfirmats[0].ToString();
        }
        else if (distribuidor.value == 3 || distribuidor.value == 0 || distribuidor.value == 6){
            param2.SetActive(true); param3.SetActive(false);
            iParam1.text = parametresConfirmats[0].ToString();
            iParam2.text = parametresConfirmats[1].ToString();

        } else {
            param2.SetActive(true); param3.SetActive(true);
            iParam1.text = parametresConfirmats[0].ToString();
            iParam2.text = parametresConfirmats[1].ToString();
            iParam3.text = parametresConfirmats[2].ToString();
        }
    }

    void Start()
    {
        generadorScript = gameObject.transform.parent.GetComponentInParent<GeneradorScript>();
        distribucioConfirmada = GeneradorScript.distribucionsProbabilitat.CONSTANT;
        politicaConfirmada = GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE;
        parametresConfirmats = new double[1];
    }

    void Update()
    {

    }
}
