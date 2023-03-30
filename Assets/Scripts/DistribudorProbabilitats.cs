using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class DistribudorProbabilitats
{
    private Triangular triangularObject = null;
    private Normal normalObject = null;
    private Exponential exponentialObject = null;
    private Poisson poissonObject = null;
    // Afegir una uniforme i una binomial

    public float constantDistribution(float seconds)
    {
        return seconds;
    }

    // DEFINES

    public void defineNormal(double normal, double sttdev){
        normalObject = new Normal(normal, sttdev);
    }

    public void defineTriangular(double lower, double upper, double middle){
        triangularObject = new Triangular(lower, upper, middle);
    }

    public void defineExponential(double rate){
        exponentialObject = new Exponential(rate);
    }

    public void definePoisson(double lambda){
        poissonObject = new Poisson(lambda);
    }

    // GETTERS

    public double getNormalSample(){
        if (normalObject == null) return -1;
        return normalObject.Sample();
    }

    public double getTriangularSample(){
        if (triangularObject == null) return -1;
        return triangularObject.Sample();
    }

    public double getExponentialSample(){
        if (exponentialObject == null) return -1;
        return exponentialObject.Sample();
    }

    public double getPoissonSample(){
        if (poissonObject == null) return -1;
        return poissonObject.Sample();
    }
    
}
