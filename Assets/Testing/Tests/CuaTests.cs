using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CuaTests
{
    public GameObject entitatTemporal;
    /*
    [Test]
    public void CuaRepUnObjecte()
    {
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        CuaScript cuaScript = cua.GetComponent<CuaScript>();
        entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        //cuaScript.recieveObject(entitatTemporal);
        // state EMPTY --> 1
        Assert.That(cuaScript.getState(), Is.EqualTo(1));
    }

    [Test]
    public void CuaEnviaObjecteIQuedaBuida()
    {
        GameObject fakeObject = new GameObject();
        fakeObject.AddComponent<FakeRebreObjecte>();
        FakeRebreObjecte fo = fakeObject.GetComponent<FakeRebreObjecte>();
        
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        CuaScript cuaScript = cua.GetComponent<CuaScript>();
        entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        cuaScript.SeguentsObjectes.Add(fakeObject);
        //cuaScript.recieveObject(entitatTemporal);
        //cuaScript.sendObject();
        
        // state EMPTY --> 0
        Assert.That(cuaScript.getState(), Is.EqualTo(0));
    }

    [Test]
    public void CuaPlenaNoDisponible()
    {
        GameObject cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        CuaScript cuaScript = cua.GetComponent<CuaScript>();
        entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        cuaScript.capacitatMaxima = 0;

        //Assert.That(cuaScript.isAvailable(this.gameObject), Is.EqualTo(false));
    }
    */
}
