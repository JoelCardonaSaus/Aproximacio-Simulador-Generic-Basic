using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProcessadorTests
{
    public GameObject entitatTemporal;
    
    [Test]
    public void ProcessadorRepUnObjecte()
    {
        GameObject processador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Processador/Processador")) as GameObject;
        ProcessadorScript processadorScript = processador.GetComponent<ProcessadorScript>();
        entitatTemporal = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        //processadorScript.distribucio = ProcessadorScript.distribucionsProbabilitat.EXPONENTIAL;
        processadorScript.distribuidor = new ExponentialDistribution(10);
        //processadorScript.parametres[0] = 10;
        //processadorScript.recieveObject(entitatTemporal);
        // state BUSY --> 1
        //Assert.That(processadorScript.getState(), Is.EqualTo(1));
    }

    [Test]
    public void ProcessadorPleNoPotRebreObjecte()
    {
        GameObject processador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Processador/Processador")) as GameObject;
        ProcessadorScript processadorScript = processador.GetComponent<ProcessadorScript>();
        processadorScript.maxEntitatsParalel = 0;

        //Assert.That(processadorScript.isAvailable(this.gameObject), Is.EqualTo(false));
    }
}
