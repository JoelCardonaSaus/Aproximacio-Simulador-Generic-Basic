using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
public class GeneradorTest
{
    public GameObject entitatTemporal;
    /*
    [Test]
    public void GeneradorEnviaUnObjecte()
    {
        
        GameObject fakeObject = new GameObject();
        fakeObject.AddComponent<FakeRebreObjecte>();
        FakeRebreObjecte fo = fakeObject.GetComponent<FakeRebreObjecte>();

        GameObject generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        GeneradorScript gs = generador.GetComponent<GeneradorScript>();
        gs.entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gs.distribucio = GeneradorScript.distribucionsProbabilitat.EXPONENTIAL;
        gs.parametres[0] = 1;
        gs.SeguentsObjectes.Add(fakeObject);
        //gs.sendObject();
        Assert.That(gs.getNGenerats(), Is.EqualTo(1));
    }

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

