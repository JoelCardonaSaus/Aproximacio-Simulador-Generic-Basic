using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

[TestFixture]
public class GeneradorTest
{
    GameObject motor;
    GameObject generador;

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        generador.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        Object.Destroy(generador);
        Object.Destroy(motor);
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
    }

    [Test]
    public void GeneradorBloquejat()
    {                
        GeneradorScript generadorScript = generador.GetComponent<GeneradorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(generadorScript.estat, Is.EqualTo(GeneradorScript.estats.BLOQUEJAT));
    }

    [Test]
    public void GeneradorEnviaUnObjecte()
    {
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.transform.parent = motor.transform;
        
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.SeguentsObjectes.Add(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(gs.getNGenerats(), Is.EqualTo(1));
        Object.Destroy(cua);
    }

    [Test]
    public void GeneradorProgramaEsdevenimentAlInstant5()
    {
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.transform.parent = motor.transform;
        
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.SeguentsObjectes.Add(cua);

 
        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(MotorSimuladorScript.Instancia.ObteTempsActual(), Is.EqualTo(5));
        Object.Destroy(cua);
    }

    
    [Test]
    public void GeneradorNoPotEnviarEntitat()
    {
        GeneradorScript generadorScript = generador.GetComponent<GeneradorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(generadorScript.getNGenerats(), Is.EqualTo(0));
    }
    
}

