using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class PoissonDistribution : ISeguentNumero
{
   
    private Poisson poissonDist = null;

    public PoissonDistribution(double lambda){
        poissonDist = new Poisson(lambda);    
    }

    public double ObteSeguentNumero(){
        return poissonDist.Sample();
    }
}
