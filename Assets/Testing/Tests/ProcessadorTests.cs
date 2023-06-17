using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProcessadorTests
{
    private GameObject motor;
    private GameObject processador;
    private GameObject cua;
    private GameObject entitatTemporalPrefab = Resources.Load<GameObject>("EntitatsTemporals/PaquetPrefab");

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        processador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Processador/Processador")) as GameObject;
        cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        processador.transform.parent = motor.transform;
        cua.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
        Object.Destroy(processador.gameObject);
        Object.Destroy(motor.gameObject);
        Object.Destroy(cua.gameObject);
    }

    [Test]
    public void ProcessadorRepEntitatIProgramaEsdevenimentAlInstant5()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();
        ps.AfegeixSeguentObjecte(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }
        Assert.AreEqual(ps.ObteEstat(), 1);
        Assert.AreEqual(MotorSimuladorScript.Instancia.ObteTempsSeguentEsdeveniment(), 5);
    }

    [Test]
    public void ProcessadorProcessaIQuedaBloquejat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.AreEqual(ps.ObteEstat(), 2);
    }

    [Test]
    public void ProcessadorProcessaIQuedaDisponible()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();
        ps.AfegeixSeguentObjecte(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.AreEqual(ps.ObteEstat(), 0);
    }

    [Test]
    public void ProcessadorBloquejatIntentaRebreEntitat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.AreEqual(ps.EstaDisponible(cua), false);
    }

    [Test]
    public void ProcessadorCapacitat1IntentaRebreSegonaEntitat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        Assert.AreEqual(ps.EstaDisponible(cua), false);
    }

    [Test]
    public void ProcessadorBloquejatNotificacioDisponibleCanviaEstat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        aux.GetComponent<CuaScript>().capacitatMaxima = 1;
        ps.GetComponent<ProcessadorScript>().NotificacioDisponible(aux);

        Assert.AreEqual(ps.ObteEstat(), 0);
    }

    [Test]
    public void ProcessadorBloquejatNotificacioDisponibleNoCanviaEstat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();
        ps.maxEntitatsParalel = 2;
        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        aux.GetComponent<CuaScript>().capacitatMaxima = 1;

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        for (int i = 0; i < 2; i++){
            if (ps.EstaDisponible(aux)){
                GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
                ps.RepEntitat(entitatAux, null);
            }
        }

        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        ps.GetComponent<ProcessadorScript>().NotificacioDisponible(aux);

        Assert.AreEqual(ps.ObteEstat(), 2);
    }

    [Test]
    public void ProcessadorProcessantNotificacioDisponibleNoCanviaEstat()
    {        
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        aux.GetComponent<CuaScript>().capacitatMaxima = 1;
        ps.GetComponent<ProcessadorScript>().NotificacioDisponible(aux);

        Assert.AreEqual(ps.ObteEstat(), 1);
    }

    
}
