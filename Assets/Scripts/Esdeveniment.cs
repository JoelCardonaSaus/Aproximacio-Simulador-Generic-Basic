using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esdeveniment
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
        productor.GetComponent<ITractarEsdeveniment>().TractarEsdeveniment(this);
    }
}
