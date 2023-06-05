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

        Assert.AreEqual(MotorSimuladorScript.Instancia.ObteTempsSeguentEsdeveniment(), 5);
    }

    [Test]
    public void ProcessadorExecutaEsdevenimentIQuedaBloquejat()
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
    public void ProcessadorExecutaEsdevenimentIQuedaDisponible()
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
    public void ProcessadorPleRebutjaObjecteAlliberaUnEspaiIRepObjecteRebutjat()
    {        
        GameObject sortida = GameObject.Instantiate(Resources.Load("LlibreriaObjectes/Sortida/Sortida")) as GameObject;
        ProcessadorScript ps = processador.GetComponent<ProcessadorScript>();
        ps.maxEntitatsParalel = 1;
        ps.AfegeixSeguentObjecte(sortida);
        cua.GetComponent<CuaScript>().AfegeixSeguentObjecte(processador);

        MotorSimuladorScript.Instancia.IniciaSimulacio();

        // S'envia una entitat i s'omple el processador
        if (ps.EstaDisponible(null)){
            GameObject entitatAux = GameObject.Instantiate(entitatTemporalPrefab);
            ps.RepEntitat(entitatAux, null);
        }

        // S'intenta enviar una de nova
        GameObject entitatAux2 = GameObject.Instantiate(entitatTemporalPrefab);
        cua.GetComponent<CuaScript>().RepEntitat(entitatAux2, null);
        if (ps.EstaDisponible(cua)) Debug.Log("No funciona be");

        // S'executa l'esdeveniment de processat, on s'allibera l'espai del processador ocupat i es demana al objecte rebutjat l'entitat temporal
        MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();

        // Es comprova que el processador esta en estat processant, la cua buida i que el processador ha enviat una entitat
        Assert.AreEqual(ps.ObteEstat(), 1);
        Assert.AreEqual(cua.GetComponent<CuaScript>().ObteEstat(), 0);
        Assert.AreEqual(ps.ObteEntitatsEnviades(), 1);

        Object.Destroy(sortida.gameObject);
    }

    /*
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
    */
}
