using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CuaTests
{
    private GameObject motor;
    private GameObject cua;
    private GameObject entitatTemporalPrefab = Resources.Load<GameObject>("EntitatsTemporals/PaquetPrefab");

    [SetUp]
    public void setup(){
        motor = GameObject.Instantiate(Resources.Load("MotorSimulador/MotorDeSimulacio")) as GameObject;
        cua = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.transform.parent = motor.transform;
    }

    [TearDown]
    public void teardown(){
        MotorSimuladorScript.Instancia.ReiniciarSimulador();
        Object.Destroy(cua.gameObject);
        Object.Destroy(motor.gameObject);
    }

    [Test]
    public void CuaRepUnaEntitatCapacitatInfinita()
    {
        cua.GetComponent<CuaScript>().capacitatMaxima = -1;
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        if (cua.GetComponent<CuaScript>().EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            cua.GetComponent<CuaScript>().RepEntitat(entitatAux, null);
        }

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(1));
    }

    [Test]
    public void CuaCapacitat1RepUnaEntitat()
    {
        GameObject generador = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Generador/Generador")) as GameObject;
        cua.GetComponent<CuaScript>().capacitatMaxima = 1;
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        if (cua.GetComponent<CuaScript>().EstaDisponible(generador)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            cua.GetComponent<CuaScript>().RepEntitat(entitatAux, generador);
        }
        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(2));
        Assert.That(cua.GetComponent<CuaScript>().EstaDisponible(generador), Is.EqualTo(false));
    }

    [Test]
    public void CuaRepUnaEntitatIEnviaAlSeguent()
    {
        GameObject cua2 = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.GetComponent<CuaScript>().AfegeixSeguentObjecte(cua2);
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        cua2.GetComponent<CuaScript>().IniciaSimulacio();
        if (cua.GetComponent<CuaScript>().EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            cua.GetComponent<CuaScript>().RepEntitat(entitatAux, null);
        }

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(0));
        Assert.That(cua.GetComponent<CuaScript>().ObteEntitatsEnviades(), Is.EqualTo(1));
    }

    [Test]
    public void CuaRepUnaEntitatINoPotEnviarAlSeguent()
    {
        GameObject cua2 = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.GetComponent<CuaScript>().AfegeixSeguentObjecte(cua2);
        cua2.GetComponent<CuaScript>().capacitatMaxima = 0;
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        cua2.GetComponent<CuaScript>().IniciaSimulacio();
        if (cua.GetComponent<CuaScript>().EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            cua.GetComponent<CuaScript>().RepEntitat(entitatAux, null);
        }

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(1));
        Assert.That(cua.GetComponent<CuaScript>().ObteEntitatsEnviades(), Is.EqualTo(0));
    }

    [Test]
    public void CuaNoBuidaNotificacioDisponible()
    {
        cua.GetComponent<CuaScript>().capacitatMaxima = 2;
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        if (cua.GetComponent<CuaScript>().EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            cua.GetComponent<CuaScript>().RepEntitat(entitatAux, null);
        }

        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.GetComponent<CuaScript>().NotificacioDisponible(aux);

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(0));
        Assert.That(cua.GetComponent<CuaScript>().ObteEntitatsEnviades(), Is.EqualTo(1));

    }

    [Test]
    public void CuaPlenaNotificacioDisponible()
    {
        cua.GetComponent<CuaScript>().capacitatMaxima = 2;
        cua.GetComponent<CuaScript>().IniciaSimulacio();
        for (int i = 0; i < 2; i++){
            if (cua.GetComponent<CuaScript>().EstaDisponible(null)){
                GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
                cua.GetComponent<CuaScript>().RepEntitat(entitatAux, null);
            }
        }

        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        aux.GetComponent<CuaScript>().capacitatMaxima = 1;
        cua.GetComponent<CuaScript>().NotificacioDisponible(aux);

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(1));
        Assert.That(cua.GetComponent<CuaScript>().ObteEntitatsEnviades(), Is.EqualTo(1));

    }

    [Test]
    public void CuaBuidaNotificacioDisponible()
    {
        cua.GetComponent<CuaScript>().capacitatMaxima = 2;
        cua.GetComponent<CuaScript>().IniciaSimulacio();

        GameObject aux = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Cua/Cua")) as GameObject;
        cua.GetComponent<CuaScript>().NotificacioDisponible(aux);

        Assert.That(cua.GetComponent<CuaScript>().ObteEstat(), Is.EqualTo(0));
        Assert.That(cua.GetComponent<CuaScript>().ObteEntitatsEnviades(), Is.EqualTo(0));

    }
   
}
