using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class NormalDistribution : ISeguentNumero
{
   
    private Normal normalDist = null;

    public NormalDistribution(double m, double sdev){
        normalDist = new Normal(m, sdev);    
    }

    public double ObteSeguentNumero(){
        return normalDist.Sample();
    }
}
