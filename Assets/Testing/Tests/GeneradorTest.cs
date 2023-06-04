using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
public class GeneradorTest
{

    [Test]
    public void GeneradorEnviaUnObjecte()
    {
        GameObject motor = MotorSimuladorScript.Instancia.gameObject;
        GameObject generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        
        generador.transform.parent = motor.transform;
        cua.transform.parent = motor.transform;
        
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.SeguentsObjectes.Add(cua);

        MotorSimuladorScript.Instancia.IniciaSimulacio();
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        Assert.That(gs.getNGenerats(), Is.EqualTo(1));
    }

    /*
    [Test]
    public void GeneradorNoEnviaCapObjecte()
    {
        GameObject fakeObject = new GameObject();
        fakeObject.AddComponent<FakeRebreObjecte>();
        FakeRebreObjecte fo = fakeObject.GetComponent<FakeRebreObjecte>();

        GameObject generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gs.SeguentsObjectes.Add(fakeObject);

        Assert.That(gs.getNGenerats(), Is.EqualTo(0));
    }
    */
}

