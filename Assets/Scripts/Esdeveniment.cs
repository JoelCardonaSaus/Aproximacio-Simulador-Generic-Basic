using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Esdeveniment : IComparable<Esdeveniment>
{

    private GameObject productor;
    private GameObject consumidor;
    public float temps;
    private GameObject entitatImplicada;

    public enum Tipus { ARRIBADES, PROCESSOS };

    public Tipus tipusEsdeveniment;

    public Esdeveniment(GameObject productor, GameObject consumidor, float temps, GameObject entitatImplicada, Tipus tipusEsdeveniment){
        this.productor = productor;
        this.consumidor = consumidor;
        this.temps = temps;
        this.entitatImplicada = entitatImplicada;
        this.tipusEsdeveniment = tipusEsdeveniment;
    }

    public void Executar(){
        consumidor.GetComponent<ITractarEsdeveniment>().TractarEsdeveniment(this);
    }

    public GameObject obteProductor()
    {
        return productor;
    }

    public GameObject obteConsumidor()
    {
        return consumidor;
    }

    public GameObject ObteEntitatImplicada();
    
    public int CompareTo(Esdeveniment other) => temps.CompareTo(other.temps);
}
