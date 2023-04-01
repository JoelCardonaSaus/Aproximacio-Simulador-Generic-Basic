using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class BinomialDistribution : ISeguentNumero
{
   
    private Binomial binomialDist = null;

    public BinomialDistribution(double p, double n){
        binomialDist = new Binomial(p, Mathf.RoundToInt((float)n));    
    }

    public double getNextSample(){
        return binomialDist.Sample();
    }
}
