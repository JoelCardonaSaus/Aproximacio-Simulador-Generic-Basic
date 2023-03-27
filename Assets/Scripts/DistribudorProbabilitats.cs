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
    private StudentT tStudentObject = null;
    private ChiSquared chiSquaredObject = null;
    private FisherSnedecor fisherObject = null;

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

    public void defineTStudent(double location, double scale, double freedom){
        tStudentObject = new StudentT(location, scale, freedom);
    }

    public void defineChiSquared(double freedom){
        chiSquaredObject = new ChiSquared(freedom);
    }

    public void defineFisher(double d1, double d2){
        fisherObject = new FisherSnedecor(d1, d2);
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

    public double getTStudentSample(){
        if (tStudentObject == null) return -1;
        return tStudentObject.Sample();
    }

    public double getChiSquaredSample(){
        if (chiSquaredObject == null) return -1;
        return chiSquaredObject.Sample();
    }

    public double getFisherSample(){
        if (fisherObject == null) return -1;
        return fisherObject.Sample();
    }
    
}
