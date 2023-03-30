using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class ExponentialDistribution : ISeguentNumero
{  
    private Exponential exponentialDist = null;

    public ExponentialDistribution(double rate){
        exponentialDist = new Exponential(rate);    
    }

    public double getNextSample(){
        return exponentialDist.Sample();
    }
}
