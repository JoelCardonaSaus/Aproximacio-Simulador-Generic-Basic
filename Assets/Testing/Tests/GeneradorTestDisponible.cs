using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;


//[TestFixture]
public class GeneradorTestDisponible
{
    private GameObject motor;
    private GameObject generador;
    private GameObject cua;

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        generador.transform.parent = motor.transform;
        cua.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
        Object.Destroy(generador.gameObject);
        Object.Destroy(motor.gameObject);
        Object.Destroy(cua.gameObject);
    }

    [Test]
    public void GeneradorEnviaUnObjecte()
    {       
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.AfegeixSeguentObjecte(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.AreEqual(gs.getNGenerats(), 1);
    }

    [Test]
    public void GeneradorProgramaEsdevenimentAlInstant5()
    {        
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.AfegeixSeguentObjecte(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        Assert.AreEqual(MotorSimuladorScript.Instancia.ObteTempsSeguentEsdeveniment(), 5);
    }

    [Test]
    public void GeneradorPassaDeBloquejatADesbloquejat()
    {
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.estat = GeneradorScript.estats.BLOQUEJAT;
        gs.NotificacioDisponible(cua);

        Assert.AreEqual(gs.estat, GeneradorScript.estats.GENERANT);
        Assert.AreEqual(gs.getNGenerats(), 1);
    }
    
}

