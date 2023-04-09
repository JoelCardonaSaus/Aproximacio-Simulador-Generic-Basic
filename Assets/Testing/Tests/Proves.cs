using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void sumTwoNumbersBad()
    {
        int a = 2;
        int b = 3;
        Assert.AreNotEqual(a+b, 4);

        // Use the Assert class to test conditions
    }

    [Test]
    public void sumTwoNumbersGood()
    {
        int a = 2;
        int b = 3;
        Assert.That(a+b, Is.EqualTo(5));

        // Use the Assert class to test conditions
    }

    [Test]
    public void substractTest()
    {
        int a = 3; 
        int b = 2;
        Assert.That(substract(a,b), Is.EqualTo(1));
    }

    public int substract(int a, int b)
    {
        return a-b;
    }

    enum states { AVAILABLE, FREE, BUSY };
    states state = states.AVAILABLE;

    [Test]
    public void testAvailable(){
        Assert.AreEqual(state, states.AVAILABLE);
    }

    public void changeStateToBusy(){
        state = states.BUSY;
    }

    [Test]
    public void testChangeState(){
        changeStateToBusy();
        Assert.AreEqual(state, states.BUSY);
        state = states.AVAILABLE;
    }

}
