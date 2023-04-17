using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class DiscreteUniformDistribution : ISeguentNumero
{
   
    private DiscreteUniform uniformDist = null;

    public DiscreteUniformDistribution(double lower, double upper){
        uniformDist = new DiscreteUniform(Mathf.RoundToInt((float)lower), Mathf.RoundToInt((float)upper));    
    }

    public double getNextSample(){
        return uniformDist.Sample();
    }
}
