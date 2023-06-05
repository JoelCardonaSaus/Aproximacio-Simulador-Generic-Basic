using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SortidaTests
{
    private GameObject motor;
    private GameObject sortida;
    private GameObject entitatTemporalPrefab = Resources.Load<GameObject>("EntitatsTemporals/PaquetPrefab");

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        sortida = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Sortida/Sortida")) as GameObject;
        sortida.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
        Object.Destroy(sortida.gameObject);
        Object.Destroy(motor.gameObject);
    }

    [Test]
    public void SortidaDestrueixUnaEntitat()
    {
        sortida.GetComponent<SortidaScript>().IniciaSimulacio();
        if (sortida.GetComponent<SortidaScript>().EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            sortida.GetComponent<SortidaScript>().RepEntitat(entitatAux, null);
        }

        Assert.That(sortida.GetComponent<SortidaScript>().getNEntitatsDestruides(), Is.EqualTo(1));
    }
}
