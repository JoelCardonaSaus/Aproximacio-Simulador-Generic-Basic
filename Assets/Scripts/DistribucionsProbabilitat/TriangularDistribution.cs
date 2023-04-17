using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class TriangularDistribution : ISeguentNumero
{
   
    private Triangular triangularDist = null;

    public TriangularDistribution(double lower, double upper, double middle){
        triangularDist = new Triangular(lower, upper, middle);    
    }

    public double getNextSample(){
        return triangularDist.Sample();
    }
}
