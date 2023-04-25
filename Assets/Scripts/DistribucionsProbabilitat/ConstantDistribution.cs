using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class ConstantDistribution : ISeguentNumero
{
   
    private double constant;

    public ConstantDistribution(double n){
        constant = n; 
    }

    public double getNextSample(){
        return constant;
    }
}
