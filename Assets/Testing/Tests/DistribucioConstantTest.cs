using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DistribucioConstantTest
{
    [Test]
    public void DistribucioConstant5SempreRetorna5()
    {
        ConstantDistribution cd = new ConstantDistribution(5);
        bool error = false;
        for (int i = 0; i < 10 && !error; i++){
            if (cd.ObteSeguentNumero() != 5) error = true;
        }

        Assert.AreEqual(error, false);
    }
}
