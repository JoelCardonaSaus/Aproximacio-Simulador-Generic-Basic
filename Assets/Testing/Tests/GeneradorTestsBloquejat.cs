using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;


//[TestFixture]
public class GeneradorTestBloquejat
{
    private GameObject motor;
    private GameObject generador;
    private GameObject cua;

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        generador.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
        Object.Destroy(generador.gameObject);
        Object.Destroy(motor.gameObject);
    }
    
    [Test]
    public void GeneradorNoPotEnviarEntitat()
    {
        GeneradorScript generadorScript = generador.GetComponent<GeneradorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(generadorScript.getNGenerats(), Is.EqualTo(0));
    }

    [Test]
    public void GeneradorEsBloqueja()
    {                
        GeneradorScript generadorScript = generador.GetComponent<GeneradorScript>();

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(generadorScript.estat, Is.EqualTo(GeneradorScript.estats.BLOQUEJAT));
    }
    
}

